using System;

using Microsoft.EntityFrameworkCore;

namespace aiof.auth.data
{
    public class AuthContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public virtual DbSet<ClientRefreshToken> ClientRefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AiofClaim> Claims { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable(Keys.Entity.User);

                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Username)
                    .IsUnique();

                e.HasQueryFilter(x => !x.IsDeleted);

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.FirstName).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.LastName).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Email).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Username).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Password).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.PrimaryApiKey).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.SecondaryApiKey).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.RoleId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.IsDeleted).HasSnakeCaseColumnName().IsRequired();

                e.HasMany(x => x.RefreshTokens)
                    .WithOne()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Client>(e =>
            {
                e.ToTable(Keys.Entity.Client);

                e.HasKey(x => x.Id);

                e.HasQueryFilter(x => x.Enabled);
                
                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Slug).HasSnakeCaseColumnName().HasMaxLength(50).IsRequired();
                e.Property(x => x.Enabled).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.PrimaryApiKey).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.SecondaryApiKey).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.RoleId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();

                e.HasMany(x => x.RefreshTokens)
                    .WithOne()
                    .HasForeignKey(x => x.ClientId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(x => x.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<UserRefreshToken>(e =>
            {
                e.ToTable(Keys.Entity.UserRefreshToken);

                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.UserId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Token).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Expires).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Revoked).HasColumnType("timestamp").HasSnakeCaseColumnName();
            });

            modelBuilder.Entity<ClientRefreshToken>(e =>
            {
                e.ToTable(Keys.Entity.ClientRefreshToken);

                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.ClientId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Token).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Expires).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Revoked).HasColumnType("timestamp").HasSnakeCaseColumnName();
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable(Keys.Entity.Role);

                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Name)
                    .IsUnique();

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<AiofClaim>(e =>
            {
                e.ToTable(Keys.Entity.Claim);

                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Name)
                    .IsUnique();

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
            });
        }
    }
}