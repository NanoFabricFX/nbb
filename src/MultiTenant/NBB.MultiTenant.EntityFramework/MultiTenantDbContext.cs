using Microsoft.EntityFrameworkCore;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace NBB.MultiTenant.EntityFramework
{
    public abstract class MultiTenantDbContext : DbContext
    {
        private readonly Tenant _tenant;
        private readonly ICryptoService _cryptoService;

        public MultiTenantDbContext(ITenantService tenantService, ICryptoService cryptoService) : base()
        {
            _cryptoService = cryptoService;
            _tenant = tenantService.GetCurrentTenant().GetAwaiter().GetResult();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_tenant.DatabaseClient == DatabaseClient.SqlClient)
            {
                optionsBuilder.UseSqlServer(_cryptoService.Decrypt(_tenant.ConnectionString));
            }
            else if (_tenant.DatabaseClient == DatabaseClient.MySql)
            {
                optionsBuilder.UseMySQL(_cryptoService.Decrypt(_tenant.ConnectionString));
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
            Expression<Func<IMayHaveTenant, bool>> optionalFilter = (IMayHaveTenant t) => t.TenantId.HasValue ? t.TenantId == _tenant.Id : true;
            Expression<Func<IMustHaveTenant, bool>> mandatoryFilter = (IMustHaveTenant t) => t.TenantId == _tenant.Id;

            var optional = modelBuilder.Model.GetEntityTypes().OfType<IMayHaveTenant>().ToList();

            optional.ForEach(t =>
            {
                var entity = modelBuilder.Entity(t.GetType());
                entity.HasQueryFilter(optionalFilter);
            });

            var mandatory = modelBuilder.Model.GetEntityTypes().OfType<IMustHaveTenant>().ToList();

            mandatory.ForEach(t =>
            {
                var entity = modelBuilder.Entity(t.GetType());
                entity.HasQueryFilter(mandatoryFilter);
            });
        }

        public override int SaveChanges()
        {
            ThrowIfMultipleTenants();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ThrowIfMultipleTenants();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfMultipleTenants();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfMultipleTenants();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void ThrowIfMultipleTenants()
        {
            if (_tenant == null)
            {
                return;
            }
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

            if (toCheck.Count == 0)
            {
                return;
            }

            if (toCheck.Count > 1)
            {
                throw new CrossTenantUpdateException(toCheck);
            }

            if (toCheck.First() != _tenant.Id)
            {
                throw new CrossTenantUpdateException(toCheck);
            }
        }
    }
}