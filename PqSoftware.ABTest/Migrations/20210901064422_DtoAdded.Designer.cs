﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PqSoftware.ABTest.Data;

namespace PqSoftware.ABTest.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20210901064422_DtoAdded")]
    partial class DtoAdded
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("PqSoftware.ABTest.Data.Models.LifetimeCount", b =>
                {
                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<int>("Lifetime")
                        .HasColumnType("integer");

                    b.ToTable("LifetimeCount", t => t.ExcludeFromMigrations());
                });

            modelBuilder.Entity("PqSoftware.ABTest.Data.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ProjectId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("PqSoftware.ABTest.Data.Models.ProjectUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateLastActivity")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateRegistration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasAlternateKey("UserId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectUsers");
                });

            modelBuilder.Entity("PqSoftware.ABTest.Data.Models.ProjectUser", b =>
                {
                    b.HasOne("PqSoftware.ABTest.Data.Models.Project", "Project")
                        .WithMany("ProjectUsers")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("PqSoftware.ABTest.Data.Models.Project", b =>
                {
                    b.Navigation("ProjectUsers");
                });
#pragma warning restore 612, 618
        }
    }
}