using Microsoft.EntityFrameworkCore;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using NBB.MultiTenant.EntityFramework.Abstractions;
using NBB.MultiTenant.EntityFramework.Extensions;
using NBB.MultiTenant.EntityFramework.Exceptions;

namespace NBB.MultiTenant.EntityFramework
{
    public abstract class MultiTenantDbContext<T> : DbContext
    {
        private readonly Tenant<T> _tenant;
        private readonly ICryptoService _cryptoService;
        private readonly ITenantService _tenantService;
        private readonly TenantOptions _tenantOptions;

        private readonly MethodInfo optionalFilterMethod = typeof(MultiTenantDbContext<T>).GetMethod("GetOptionalFilter");
        private readonly MethodInfo mandatoryFilterMethod = typeof(MultiTenantDbContext<T>).GetMethod("GetMandatoryFilter");
        private readonly MethodInfo setDefaultValueMethod = typeof(MultiTenantDbContext<T>).GetMethod("SetDefaultValue");

        public MultiTenantDbContext(ICryptoService cryptoService, ITenantService tenantService, TenantOptions tenantOptions)
        {
            _cryptoService = cryptoService;
            _tenantService = tenantService;
            _tenantOptions = tenantOptions;

            _tenant = _tenantService.GetCurrentTenant<T>();

            if (_tenantOptions.IsReadOnly)
            {
                ChangeTracker.AutoDetectChangesEnabled = false;
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        #region base

        public Expression<Func<T, bool>> GetMandatoryFilter<T>(ModelBuilder modelBuilder, T tenantId) where T : class, IMustHaveTenant<T>
        {
            Expression<Func<T, bool>> filter = t => t.TenantId.Equals(tenantId);
            return filter;
        }

        public Expression<Func<T, bool>> GetOptionalFilter<T>(ModelBuilder modelBuilder, Guid tenantId) where T : class, IMayHaveTenant<T>
        {
            Expression<Func<T, bool>> filter = t => !t.TenantId.IsNullOrDefault();
            return filter;
        }

        public void SetDefaultValue<T>(ModelBuilder modelBuilder, Guid tenantId) where T : class, IMustHaveTenant<T>
        {
            modelBuilder.Entity<T>().Property(nameof(IMustHaveTenant<T>.TenantId)).HasDefaultValue(tenantId);
        }

        protected void ApplyDefaultValues(ModelBuilder modelBuilder)
        {
            if (_tenant == null)
            {
                return;
            }
            var mandatory = new List<IMutableEntityType>();

            mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMustHaveTenant<T>).IsAssignableFrom(p.ClrType)).ToList());

            mandatory = mandatory.Distinct().ToList();

            var tenantId = _tenant.TenantId;

            mandatory.ToList().ForEach(t =>
            {
                var generic = setDefaultValueMethod.MakeGenericMethod(t.ClrType);
                var expr = generic.Invoke(this, new object[] { modelBuilder, tenantId });
            });
        }

        protected void ApplyDefaultValues(ModelBuilder modelBuilder, List<IMutableEntityType> optional, List<IMutableEntityType> mandatory)
        {
            if (_tenant == null)
            {
                return;
            }
            var tenantId = _tenant.TenantId;

            mandatory.ToList().ForEach(t =>
            {
                var generic = setDefaultValueMethod.MakeGenericMethod(t.ClrType);
                var expr = generic.Invoke(this, new object[] { modelBuilder, tenantId });
            });
        }

        protected virtual void AddQueryFilters(ModelBuilder modelBuilder, List<IMutableEntityType> optional, List<IMutableEntityType> mandatory)
        {
            if (_tenant == null)
            {
                return;
            }

            var tenantId = _tenant.TenantId;

            optional.ForEach(t =>
            {
                var generic = optionalFilterMethod.MakeGenericMethod(t.ClrType);
                var expr = generic.Invoke(this, new object[] { modelBuilder, tenantId });
                modelBuilder.Entity(t.ClrType).HasQueryFilter((LambdaExpression)expr);
            });

            mandatory.ToList().ForEach(t =>
            {
                var generic = mandatoryFilterMethod.MakeGenericMethod(t.ClrType);
                var expr = generic.Invoke(this, new object[] { modelBuilder, tenantId });
                modelBuilder.Entity(t.ClrType).HasQueryFilter((LambdaExpression)expr);
            });
        }

        protected List<T> GetViolationsByInheritance()
        {
            var optionalIds = (from e in ChangeTracker.Entries()
                               where e.Entity is IMayHaveTenant<T> && !((IMayHaveTenant<T>)e.Entity).TenantId.IsNullOrDefault()
                               select ((IMayHaveTenant<T>)e.Entity).TenantId)
                       .Distinct()
                       .ToList();

            var mandatoryIds = (from e in ChangeTracker.Entries()
                                where e.Entity is IMustHaveTenant<T>
                                select ((IMustHaveTenant<T>)e.Entity).TenantId)
                       .Distinct()
                       .ToList();

            var toCheck = optionalIds.Union(mandatoryIds).ToList();
            return toCheck;
        }

        protected void UpdateDefaultTenantId()
        {
            if (_tenant == null)
            {
                return;
            }

            var list = ChangeTracker.Entries()
                .Where(e => e.Entity is IMustHaveTenant<T> && ((IMustHaveTenant<T>)e.Entity).TenantId.IsNullOrDefault<T>())
                .Select(e => ((IMustHaveTenant<T>)e.Entity));
            foreach (var e in list)
            {
                e.TenantId = _tenant.TenantId;
            }
        }

        protected List<T> GetViolations()
        {
            var list = GetViolationsByInheritance();

            return list.Distinct().ToList();
        }

        protected void ThrowIfMultipleTenants()
        {
            if (_tenant == null)
            {
                return;
            }

            var toCheck = GetViolations();

            if (toCheck.Count >= 1 && _tenantOptions.IsReadOnly)
            {
                throw new Exception("Read only Db context");
            }

            if (toCheck.Count == 0)
            {
                return;
            }

            if (!_tenantOptions.RestrictCrossTenantAccess)
            {
                return;
            }

            if (toCheck.Count > 1)
            {
                throw new CrossTenantUpdateException<T>(toCheck);
            }

            if (!toCheck.First().Equals(_tenant.TenantId))
            {
                throw new CrossTenantUpdateException<T>(toCheck);
            }
        }

        #endregion       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_tenant.DatabaseClient == DatabaseClient.SqlClient)
            {
                optionsBuilder.UseSqlServer(_cryptoService.Decrypt(_tenant.ConnectionString));
            }
            else
            {
                throw new Exception($"Unsupported database type {_tenant.DatabaseClient}");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_tenant == null)
            {
                return;
            }
            if (_tenantOptions.UseDefaultValueOnInsert)
            {
                ApplyDefaultValues(modelBuilder);
            }

            if (!_tenantOptions.RestrictCrossTenantAccess)
            {
                base.OnModelCreating(modelBuilder);
                return;
            }
            var optional = new List<IMutableEntityType>();
            var mandatory = new List<IMutableEntityType>();
            optional.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMayHaveTenant<T>).IsAssignableFrom(p.GetType())).ToList());
            mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMustHaveTenant<T>).IsAssignableFrom(p.ClrType)).ToList());

            optional = optional.Distinct().ToList();
            mandatory = mandatory.Distinct().ToList();

            AddQueryFilters(modelBuilder, optional, mandatory);


            base.OnModelCreating(modelBuilder);
        }

        #region Write side
        public override int SaveChanges()
        {
            if (_tenant == null)
            {
                return base.SaveChanges();
            }

            if (_tenantOptions.IsReadOnly)
            {
                throw new Exception("Readonly");
            }

            if (_tenantOptions.UseDefaultValueOnInsert)
            {
                UpdateDefaultTenantId();
            }

            if (_tenantOptions.RestrictCrossTenantAccess)
            {
                ThrowIfMultipleTenants();
            }

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (_tenant == null)
            {
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }

            if (_tenantOptions.IsReadOnly)
            {
                throw new Exception("Readonly");
            }

            if (_tenantOptions.UseDefaultValueOnInsert)
            {
                UpdateDefaultTenantId();
            }

            if (_tenantOptions.RestrictCrossTenantAccess)
            {
                ThrowIfMultipleTenants();
            }

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_tenant == null)
            {
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            if (_tenantOptions.IsReadOnly)
            {
                throw new Exception("Readonly");
            }

            if (_tenantOptions.UseDefaultValueOnInsert)
            {
                UpdateDefaultTenantId();
            }

            if (_tenantOptions.RestrictCrossTenantAccess)
            {
                ThrowIfMultipleTenants();
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_tenant == null)
            {
                return base.SaveChangesAsync(cancellationToken);
            }

            if (_tenantOptions.IsReadOnly)
            {
                throw new Exception("Readonly");
            }

            if (_tenantOptions.UseDefaultValueOnInsert)
            {
                UpdateDefaultTenantId();
            }

            if (_tenantOptions.RestrictCrossTenantAccess)
            {
                ThrowIfMultipleTenants();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

    }
}