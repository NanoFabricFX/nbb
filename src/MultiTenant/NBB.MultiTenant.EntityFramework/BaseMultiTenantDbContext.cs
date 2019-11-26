using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NBB.MultiTenant.EntityFramework
{
    public abstract class BaseMultiTenantDbContext<T> : DbContext
    {
        private readonly TenantOptions _tenantOptions;
        readonly MethodInfo optionalFilterMethod = typeof(ReadOnlyMultitenantDbContext<T>).GetMethod("GetOptionalFilter");
        readonly MethodInfo mandatoryFilterMethod = typeof(ReadOnlyMultitenantDbContext<T>).GetMethod("GetMandatoryFilter");
        readonly MethodInfo setDefaultValueMethod = typeof(ReadOnlyMultitenantDbContext<T>).GetMethod("SetDefaultValue");

        private readonly Tenant<T> _tenant;

        public BaseMultiTenantDbContext(TenantOptions tenantOptions, ITenantService tenantService)
        {
            _tenantOptions = tenantOptions;
            _tenant = tenantService.GetCurrentTenant<T>();
        }

        public Expression<Func<T, bool>> GetMandatoryFilter<T>(ModelBuilder modelBuilder, T tenantId) where T : class, IMustHaveTenant<T>
        {
            Expression<Func<T, bool>> filter = t => t.TenantId == tenantId;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_tenant == null)
            {
                return;
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

            //if (_tenantOptions.UseDatabaseInheritance)
            //{
            //    optional.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMayHaveTenant<T>).IsAssignableFrom(p.GetType())).ToList());
            //    mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMustHaveTenant<T>).IsAssignableFrom(p.ClrType)).ToList());
            //}
            //if (_tenantOptions.UseDatabaseAnnotations)
            //{
            //    optional.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => p.FindAnnotation(nameof(IMayHaveTenant<T>)) != null && Convert.ToBoolean(p.FindAnnotation(nameof(IMayHaveTenant<T>)).Value)));
            //    mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => p.FindAnnotation(nameof(IMustHaveTenant<T>)) != null && Convert.ToBoolean(p.FindAnnotation(nameof(IMustHaveTenant<T>)).Value)));
            //}
            optional = optional.Distinct().ToList();
            mandatory = mandatory.Distinct().ToList();
            AddQueryFilters(modelBuilder, optional, mandatory, _tenant);
            base.OnModelCreating(modelBuilder);
        }

        protected void ApplyDefaultValues(ModelBuilder modelBuilder)
        {
            if (_tenant == null)
            {
                return;
            }
            var mandatory = new List<IMutableEntityType>();

            mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMustHaveTenant<T>).IsAssignableFrom(p.ClrType)).ToList());

            //if (_tenantOptions.UseDatabaseAnnotations)
            //{
            //    mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => p.FindAnnotation("IMustHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMustHaveTenant").Value)));
            //}
            mandatory = mandatory.Distinct().ToList();

            var tenantId = _tenant.TenantId;

            mandatory.ToList().ForEach(t =>
            {
                var generic = setDefaultValueMethod.MakeGenericMethod(t.ClrType);
                var expr = generic.Invoke(this, new object[] { modelBuilder, tenantId });
            });
        }

        protected void ApplyDefaultValues(ModelBuilder modelBuilder, List<IMutableEntityType> optional, List<IMutableEntityType> mandatory, Tenant<T> tenant)
        {
            var tenantId = tenant.TenantId;

            mandatory.ToList().ForEach(t =>
            {
                var generic = setDefaultValueMethod.MakeGenericMethod(t.ClrType);
                var expr = generic.Invoke(this, new object[] { modelBuilder, tenantId });
            });
        }

        protected virtual void AddQueryFilters(ModelBuilder modelBuilder, List<IMutableEntityType> optional, List<IMutableEntityType> mandatory, Tenant<T> tenant)
        {
            var tenantId = tenant.TenantId;

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

        protected List<T> GetViolationsByAnnotations()
        {
            // Get all the entity types information contained in the DbContext class, ...
            var mandatory = Model
                .GetEntityTypes()
                .Where(p => p.FindAnnotation("IMustHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMustHaveTenant").Value))
                .Select(x => x.ClrType)
                .ToList();

            var optional = Model
                .GetEntityTypes()
                .Where(p => p.FindAnnotation("IMightHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMightHaveTenant").Value)) // findproperty din IEntityType. asa crapa
                .Select(x => x.ClrType)
                .ToList();

            var optionalIds = (from e in ChangeTracker.Entries()
                               where optional.Contains(e.Entity.GetType()) && !((IMayHaveTenant<T>)e.Entity).TenantId.IsNullOrDefault<T>()
                               select ((IMayHaveTenant<T>)e.Entity).TenantId)
                      .Distinct()
                      .ToList();

            var mandatoryIds = (from e in ChangeTracker.Entries()
                                where mandatory.Contains(e.Entity.GetType())
                                select ((IMustHaveTenant<T>)e.Entity).TenantId)
                       .Distinct()
                       .ToList();

            var toCheck = optionalIds.Union(mandatoryIds).ToList();
            return toCheck;
        }

        protected void UpdateDefaultTenantId(Tenant<T> tenant)
        {
            if (tenant == null)
            {
                return;
            }

            var list = ChangeTracker.Entries()
                .Where(e => e.Entity is IMustHaveTenant<T> && ((IMustHaveTenant<T>)e.Entity).TenantId.IsNullOrDefault<T>())
                .Select(e => ((IMustHaveTenant<T>)e.Entity));
            foreach (var e in list)
            {
                e.TenantId = tenant.TenantId;
            }
        }

        protected List<T> GetViolations()
        {
            var list = GetViolationsByInheritance();

            return list.Distinct().ToList();
        }

        protected void ThrowIfMultipleTenants(Tenant<T> tenant)
        {
            if (tenant == null)
            {
                return;
            }

            if (!_tenantOptions.RestrictCrossTenantAccess)
            {
                return;
            }

            List<T> toCheck = GetViolations();

            if (toCheck.Count == 0)
            {
                return;
            }

            if (toCheck.Count > 1)
            {
                throw new CrossTenantUpdateException<T>(toCheck);
            }

            if (!toCheck.First().Equals(tenant.TenantId))
            {
                throw new CrossTenantUpdateException<T>(toCheck);
            }
        }
    }
}