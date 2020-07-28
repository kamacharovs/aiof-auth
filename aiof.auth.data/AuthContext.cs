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
                e.Property(x => x.PublicKey).HasColumnName("public_key").IsRequired();
                e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(200).IsRequired();
                e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(200).IsRequired();
                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200).IsRequired();
                e.Property(x => x.Username).HasColumnName("username").HasMaxLength(200).IsRequired();
                e.Property(x => x.Password).HasColumnName("password").HasMaxLength(100).IsRequired();
                e.Property(x => x.Created).HasColumnType("date").IsRequired();
            });

            modelBuilder.Entity<Client>(e =>
            {
                e.ToTable("client");

                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Slug)
                    .IsUnique();

                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasColumnName("public_key").IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
                e.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(50).IsRequired();
                e.Property(x => x.Enabled).HasColumnName("enabled").IsRequired();
                e.Property(x => x.PrimaryApiKey).HasColumnName("primary_api_key").HasMaxLength(100);
                e.Property(x => x.SecondaryApiKey).HasColumnName("secondary_api_key").HasMaxLength(100);
                e.Property(x => x.Created).HasColumnType("date").HasColumnName("created").IsRequired();
            });

            modelBuilder.Entity<ClientRefreshToken>(e =>
            {
                e.ToTable("client_refresh_token");

                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasColumnName("public_key").IsRequired();
                e.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
                e.Property(x => x.Token).HasColumnName("token").HasMaxLength(100).IsRequired();
                e.Property(x => x.Created).HasColumnType("date").HasColumnName("created").IsRequired();
                e.Property(x => x.Expires).HasColumnType("date").HasColumnName("expires").IsRequired();
                e.Property(x => x.Revoked).HasColumnType("date").HasColumnName("revoked");

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

                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
                e.Property(x => x.PublicKey).HasColumnName("public_key").IsRequired();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            });
        }
    }
}