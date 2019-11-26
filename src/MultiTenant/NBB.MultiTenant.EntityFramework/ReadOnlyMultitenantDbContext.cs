using Microsoft.EntityFrameworkCore;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace NBB.MultiTenant.EntityFramework
{
    public class ReadOnlyMultitenantDbContext<T> : BaseMultiTenantDbContext<T>
    {
        private readonly Tenant<T> _tenant;
        private readonly ICryptoService _cryptoService;
        private readonly ITenantService _tenantService;
        private readonly string _connectionString = null;
        private readonly TenantOptions _tenantOptions;

        public ReadOnlyMultitenantDbContext(ICryptoService cryptoService, ITenantService tenantService, TenantOptions tenantOptions) : base(tenantOptions, tenantService)
        {
            _cryptoService = cryptoService;
            _tenantService = tenantService;
            _tenantOptions = tenantOptions;

            _tenant = _tenantService.GetCurrentTenant<T>();
            _connectionString = _tenant.ConnectionString;

            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (_tenant.DatabaseClient == DatabaseClient.SqlClient)
            {
                optionsBuilder.UseSqlServer(_cryptoService.Decrypt(_connectionString));
            }
            //else if (_tenant.DatabaseClient == DatabaseClient.MySql)
            //{
            //    optionsBuilder.UseMySQL(_cryptoService.Decrypt(_tenant.ConnectionString));
            //}
            else
            {
                throw new Exception($"Unsupported database type {_tenant.DatabaseClient}");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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