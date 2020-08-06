using System;

using Microsoft.EntityFrameworkCore;

namespace aiof.auth.data
{
    public class AuthContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientRefreshToken> ClientRefreshTokens { get; set; }
        public virtual DbSet<AiofClaim> Claims { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("user");

                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Username)
                    .IsUnique();

                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.FirstName).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.LastName).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Email).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Username).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Password).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
            });

            modelBuilder.Entity<Client>(e =>
            {
                e.ToTable("client");

                e.HasKey(x => x.Id);
                
                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Name).HasSnakeCaseColumnName().HasMaxLength(200).IsRequired();
                e.Property(x => x.Slug).HasSnakeCaseColumnName().HasMaxLength(50).IsRequired();
                e.Property(x => x.Enabled).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.PrimaryApiKey).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.SecondaryApiKey).HasSnakeCaseColumnName().HasMaxLength(100);
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
            });

            modelBuilder.Entity<ClientRefreshToken>(e =>
            {
                e.ToTable("client_refresh_token");

                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasSnakeCaseColumnName().ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.ClientId).HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Token).HasSnakeCaseColumnName().HasMaxLength(100).IsRequired();
                e.Property(x => x.Created).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Expires).HasColumnType("timestamp").HasSnakeCaseColumnName().IsRequired();
                e.Property(x => x.Revoked).HasColumnType("timestamp").HasSnakeCaseColumnName();

                e.HasOne(x => x.Client)
                    .WithMany()
                    .HasForeignKey(x => x.ClientId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AiofClaim>(e =>
            {
                e.ToTable("claim");

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