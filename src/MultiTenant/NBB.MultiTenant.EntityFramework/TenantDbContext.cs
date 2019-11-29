using Microsoft.EntityFrameworkCore;
using NBB.MultiTenant.EntityFramework.Entities;

namespace NBB.MultiTenant.EntityFramework
{
    public partial class TenantDbContext<T> : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext<T>> options)
            : base(options)
        {
        }

        protected TenantDbContext(DbContextOptions options): base(options)
        {

        }

        public virtual DbSet<Feature<T>> Features { get; set; }
        public virtual DbSet<FeatureUserRight<T>> FeatureUserRights { get; set; }
        public virtual DbSet<Role<T>> Roles { get; set; }
        public virtual DbSet<RoleUserRight<T>> RoleUserRights { get; set; }
        public virtual DbSet<Subscription<T>> Subscriptions { get; set; }
        public virtual DbSet<SubscriptionFeature<T>> SubscriptionFeatures { get; set; }
        public virtual DbSet<Tenant<T>> Tenants { get; set; }
        public virtual DbSet<TenantFeature<T>> TenantFeatures { get; set; }
        public virtual DbSet<TenantSubcription<T>> TenantSubcriptions { get; set; }
        public virtual DbSet<TenantUser<T>> TenantUsers { get; set; }
        public virtual DbSet<User<T>> Users { get; set; }
        public virtual DbSet<UserFeature<T>> UserFeatures { get; set; }
        public virtual DbSet<UserRight<T>> UserRights { get; set; }
        public virtual DbSet<UserRole<T>> UserRoles { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Feature<T>>(entity =>
            {
                entity.HasKey(e => e.FeatureId);
                entity.Property(e => e.FeatureId);//.HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<FeatureUserRight<T>>(entity =>
            {
                entity.HasKey(e => new { e.FeatureId, e.UserRightId });

                entity.HasOne(d => d.Feature)
                    .WithMany(p => p.FeatureUserRights)
                    .HasForeignKey(d => d.FeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeatureUserRights_Features");

                entity.HasOne(d => d.UserRight)
                    .WithMany(p => p.FeatureUserRights)
                    .HasForeignKey(d => d.UserRightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeatureUserRights_UserRights");
            });

            modelBuilder.Entity<Role<T>>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId);//.HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RoleUserRight<T>>(entity =>
            {
                entity.HasKey(e => new { e.UserRightId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleUserRights)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleUserRights_Roles");

                entity.HasOne(d => d.UserRight)
                    .WithMany(p => p.RoleUserRights)
                    .HasForeignKey(d => d.UserRightId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleUserRights_UserRights");
            });

            modelBuilder.Entity<Subscription<T>>(entity =>
            {
                entity.HasKey(e => e.SubscriptionId);
                entity.Property(e => e.SubscriptionId);//.HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SubscriptionFeature<T>>(entity =>
            {
                entity.HasKey(e => new { e.SubcriptionId, e.FeatureId });

                entity.Property(e => e.FeatureValue).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Feature)
                    .WithMany(p => p.SubscriptionFeatures)
                    .HasForeignKey(d => d.FeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubscriptionFeatures_Features");

                entity.HasOne(d => d.Subcription)
                    .WithMany(p => p.SubscriptionFeatures)
                    .HasForeignKey(d => d.SubcriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubscriptionFeatures_Subscriptions");
            });

            modelBuilder.Entity<Tenant<T>>(entity =>
            {
                entity.HasKey(e => e.TenantId);
                entity.Property(e => e.TenantId);//.HasDefaultValueSql("(newid())");

                entity.Property(e => e.ConnectionString).IsRequired();

                entity.Property(e => e.Host).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.SourceIp).HasMaxLength(40);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Tenants)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tenants_Users");
            });

            modelBuilder.Entity<TenantFeature<T>>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.FeatureId });

                entity.Property(e => e.FeatureValue).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Feature)
                    .WithMany(p => p.TenantFeatures)
                    .HasForeignKey(d => d.FeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantFeatures_Features");

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.TenantFeatures)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantFeatures_Tenants");
            });

            modelBuilder.Entity<TenantSubcription<T>>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.SubcriptionId, e.StartDate });

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.HasOne(d => d.Subcription)
                    .WithMany(p => p.TenantSubcriptions)
                    .HasForeignKey(d => d.SubcriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantSubcriptions_Subscriptions");

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.TenantSubcriptions)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantSubcriptions_Tenants");
            });

            modelBuilder.Entity<TenantUser<T>>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.UserId });

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.TenantUsers)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantUsers_Tenant");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TenantUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TenantUsers_Users");
            });

            modelBuilder.Entity<User<T>>(entity =>
            {
                //entity.HasIndex(e => e.Email)
                //    .HasName("iUsers_Email")
                //    .IsUnique();

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId);//.HasDefaultValueSql("(newid())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<UserFeature<T>>(entity =>
            {
                entity.HasKey(e => new { e.FeatureId, e.UserId });

                entity.Property(e => e.FeatureValue).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Feature)
                    .WithMany(p => p.UserFeatures)
                    .HasForeignKey(d => d.FeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeatures_Features");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFeatures)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFeatures_Users");
            });

            modelBuilder.Entity<UserRight<T>>(entity =>
            {
                entity.HasKey(x => x.UserRightId);
                entity.Property(e => e.UserRightId);//.HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserRole<T>>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId, e.TenantId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Tenant");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}