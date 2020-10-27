﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using aiof.auth.data;

namespace aiof.auth.data.Migrations
{
    [DbContext(typeof(AuthContext))]
    [Migration("20201027154606_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("aiof.auth.data.AiofClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<Guid>("PublicKey")
                        .HasColumnName("public_key")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("claim");
                });

            modelBuilder.Entity("aiof.auth.data.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp");

                    b.Property<bool>("Enabled")
                        .HasColumnName("enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<string>("PrimaryApiKey")
                        .IsRequired()
                        .HasColumnName("primary_api_key")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<Guid>("PublicKey")
                        .HasColumnName("public_key")
                        .HasColumnType("uuid");

                    b.Property<int>("RoleId")
                        .HasColumnName("role_id")
                        .HasColumnType("integer");

                    b.Property<string>("SecondaryApiKey")
                        .IsRequired()
                        .HasColumnName("secondary_api_key")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnName("slug")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("client");
                });

            modelBuilder.Entity("aiof.auth.data.ClientRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ClientId")
                        .HasColumnName("client_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("Expires")
                        .HasColumnName("expires")
                        .HasColumnType("timestamp");

                    b.Property<Guid>("PublicKey")
                        .HasColumnName("public_key")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnName("revoked")
                        .HasColumnType("timestamp");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnName("token")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("client_refresh_token");
                });

            modelBuilder.Entity("aiof.auth.data.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<Guid>("PublicKey")
                        .HasColumnName("public_key")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("role");
                });

            modelBuilder.Entity("aiof.auth.data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnName("first_name")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnName("last_name")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnName("password")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("PrimaryApiKey")
                        .HasColumnName("primary_api_key")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<Guid>("PublicKey")
                        .HasColumnName("public_key")
                        .HasColumnType("uuid");

                    b.Property<int>("RoleId")
                        .HasColumnName("role_id")
                        .HasColumnType("integer");

                    b.Property<string>("SecondaryApiKey")
                        .HasColumnName("secondary_api_key")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnName("username")
                        .HasColumnType("character varying(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("aiof.auth.data.UserRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("timestamp");

                    b.Property<DateTime>("Expires")
                        .HasColumnName("expires")
                        .HasColumnType("timestamp");

                    b.Property<Guid>("PublicKey")
                        .HasColumnName("public_key")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnName("revoked")
                        .HasColumnType("timestamp");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnName("token")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<int>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("user_refresh_token");
                });

            modelBuilder.Entity("aiof.auth.data.Client", b =>
                {
                    b.HasOne("aiof.auth.data.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();
                });

            modelBuilder.Entity("aiof.auth.data.ClientRefreshToken", b =>
                {
                    b.HasOne("aiof.auth.data.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("aiof.auth.data.User", b =>
                {
                    b.HasOne("aiof.auth.data.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();
                });

            modelBuilder.Entity("aiof.auth.data.UserRefreshToken", b =>
                {
                    b.HasOne("aiof.auth.data.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
