using Microsoft.EntityFrameworkCore;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace NBB.MultiTenant.EntityFramework
{
    public class ReadOnlyMultitenantDbContext : DbContext
    {
        private Tenant _tenant;
        private readonly ICryptoService _cryptoService;

        public ReadOnlyMultitenantDbContext(ITenantService tenantService, ICryptoService cryptoService) : base()
        {
            _cryptoService = cryptoService;
            _tenant = tenantService.GetCurrentTenant().GetAwaiter().GetResult();
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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
            throw new Exception("Read only context");
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new Exception("Read only context");
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new Exception("Read only context");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new Exception("Read only context");
        }
    }
}