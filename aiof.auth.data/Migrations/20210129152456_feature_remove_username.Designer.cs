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
    [Migration("20210129152456_feature_remove_username")]
    partial class feature_remove_username
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("aiof.auth.data.AiofClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("claim");
                });

            modelBuilder.Entity("aiof.auth.data.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("created");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean")
                        .HasColumnName("enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<string>("PrimaryApiKey")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("primary_api_key");

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<string>("SecondaryApiKey")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("secondary_api_key");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("slug");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("client");
                });

            modelBuilder.Entity("aiof.auth.data.ClientRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("ClientId")
                        .HasColumnType("integer")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("created");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp")
                        .HasColumnName("expires");

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("timestamp")
                        .HasColumnName("revoked");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("token");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("client_refresh_token");
                });

            modelBuilder.Entity("aiof.auth.data.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("role");
                });

            modelBuilder.Entity("aiof.auth.data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("created");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("password");

                    b.Property<string>("PrimaryApiKey")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("primary_api_key");

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.Property<string>("SecondaryApiKey")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("secondary_api_key");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("user");
                });

            modelBuilder.Entity("aiof.auth.data.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.ToTable("user_profile");
                });

            modelBuilder.Entity("aiof.auth.data.UserRefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp")
                        .HasColumnName("created");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp")
                        .HasColumnName("expires");

                    b.Property<Guid>("PublicKey")
                        .HasColumnType("uuid")
                        .HasColumnName("public_key");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("timestamp")
                        .HasColumnName("revoked");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("token");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

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

                    b.Navigation("Role");
                });

            modelBuilder.Entity("aiof.auth.data.ClientRefreshToken", b =>
                {
                    b.HasOne("aiof.auth.data.Client", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();
                });

            modelBuilder.Entity("aiof.auth.data.User", b =>
                {
                    b.HasOne("aiof.auth.data.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("aiof.auth.data.UserRefreshToken", b =>
                {
                    b.HasOne("aiof.auth.data.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();
                });

            modelBuilder.Entity("aiof.auth.data.Client", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("aiof.auth.data.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
