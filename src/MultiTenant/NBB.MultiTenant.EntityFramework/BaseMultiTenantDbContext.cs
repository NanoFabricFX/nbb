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
    public abstract class BaseMultiTenantDbContext : DbContext
    {
        private readonly TenantOptions _tenantOptions;
        MethodInfo optionalFilterMethod = typeof(ReadOnlyMultitenantDbContext).GetMethod("GetOptionalFilter");
        MethodInfo mandatoryFilterMethod = typeof(ReadOnlyMultitenantDbContext).GetMethod("GetMandatoryFilter");
        private readonly ITenantService _tenantService;
        private readonly NBB.MultiTenant.Abstractions.Tenant _tenant;

        public BaseMultiTenantDbContext(TenantOptions tenantOptions, ITenantService tenantService)
        {
            _tenantOptions = tenantOptions;
            _tenantService = tenantService;
            _tenant = tenantService.GetCurrentTenant().GetAwaiter().GetResult();
        }

        public Expression<Func<T, bool>> GetMandatoryFilter<T>(ModelBuilder modelBuilder, Guid tenantId) where T : class, IMustHaveTenant
        {
            Expression<Func<T, bool>> filter = t => t.TenantId == tenantId;
            return filter;
        }

        public Expression<Func<T, bool>> GetOptionalFilter<T>(ModelBuilder modelBuilder, Guid tenantId) where T : class, IMayHaveTenant
        {
            Expression<Func<T, bool>> filter = t => t.TenantId.HasValue ? t.TenantId == tenantId : true;
            return filter;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_tenant == null)
            {
                return;
            }
            var optional = new List<IMutableEntityType>();
            var mandatory = new List<IMutableEntityType>();
            if (_tenantOptions.UseDatabaseInheritance)
            {
                optional.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMayHaveTenant).IsAssignableFrom(p.GetType())).ToList());
                mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => typeof(IMustHaveTenant).IsAssignableFrom(p.ClrType)).ToList());
            }
            if (_tenantOptions.UseDatabaseAnnotations)
            {
                optional.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => p.FindAnnotation("IMayHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMayHaveTenant").Value)));
                mandatory.AddRange(modelBuilder.Model.GetEntityTypes().Where(p => p.FindAnnotation("IMustHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMustHaveTenant").Value)));
            }
            optional = optional.Distinct().ToList();
            mandatory = mandatory.Distinct().ToList();
            AddQueryFilters(modelBuilder, optional, mandatory, _tenant);
        }

        protected virtual void AddQueryFilters(ModelBuilder modelBuilder, List<IMutableEntityType> optional, List<IMutableEntityType> mandatory, NBB.MultiTenant.Abstractions.Tenant tenant)
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

        protected List<Guid> GetViolationsByInheritance()
        {
            var optionalIds = (from e in ChangeTracker.Entries()
                               where e.Entity is IMayHaveTenant && ((IMayHaveTenant)e.Entity).TenantId.HasValue
                               select ((IMayHaveTenant)e.Entity).TenantId.Value)
                       .Distinct()
                       .ToList();

            var mandatoryIds = (from e in ChangeTracker.Entries()
                                where e.Entity is IMustHaveTenant
                                select ((IMustHaveTenant)e.Entity).TenantId)
                       .Distinct()
                       .ToList();

            var toCheck = optionalIds.Union(mandatoryIds).ToList();
            return toCheck;
        }

        protected List<Guid> GetViolationsByAnnotations()
        {
            // Get all the entity types information contained in the DbContext class, ...
            var mandatory = Model
                .GetEntityTypes()
                .Where(p => p.FindAnnotation("IMustHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMustHaveTenant").Value))
                .Select(x => x.ClrType)
                .ToList();

            var optional = Model
                .GetEntityTypes()
                .Where(p => p.FindAnnotation("IMightHaveTenant") != null && Convert.ToBoolean(p.FindAnnotation("IMightHaveTenant").Value))
                .Select(x => x.ClrType)
                .ToList();

            var optionalIds = (from e in ChangeTracker.Entries()
                               where optional.Contains(e.Entity.GetType()) && ((IMayHaveTenant)e.Entity).TenantId.HasValue
                               select ((IMayHaveTenant)e.Entity).TenantId.Value)
                      .Distinct()
                      .ToList();

            var mandatoryIds = (from e in ChangeTracker.Entries()
                                where mandatory.Contains(e.Entity.GetType())
                                select ((IMustHaveTenant)e.Entity).TenantId)
                       .Distinct()
                       .ToList();

            var toCheck = optionalIds.Union(mandatoryIds).ToList();
            return toCheck;
        }

        protected List<Guid> GetViolations()
        {
            var list = new List<Guid>();

            if (_tenantOptions.UseDatabaseInheritance)
            {
                list.AddRange(GetViolationsByInheritance());
            }
            if (_tenantOptions.UseDatabaseAnnotations)
            {
                list.AddRange(GetViolationsByAnnotations());
            }

            return list.Distinct().ToList();
        }

        protected void ThrowIfMultipleTenants(NBB.MultiTenant.Abstractions.Tenant tenant)
        {
            if (tenant == null)
            {
                return;
            }

            if (!_tenantOptions.RestrictCrossTenantAccess)
            {
                return;
            }

            List<Guid> toCheck = new List<Guid>();

            if (_tenantOptions.UseDatabaseInheritance)
            {
                toCheck = GetViolationsByInheritance();
            }

            if (_tenantOptions.UseDatabaseAnnotations)
            {
                toCheck.AddRange(GetViolationsByAnnotations());
            }

            if (toCheck.Count == 0)
            {
                return;
            }

            if (toCheck.Count > 1)
            {
                throw new CrossTenantUpdateException(toCheck);
            }

            if (toCheck.First() != tenant.TenantId)
            {
                throw new CrossTenantUpdateException(toCheck);
            }
        }
    }
}