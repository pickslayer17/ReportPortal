﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReportPortal.DAL;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("Name");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("ParentId");

                    b.Property<int?>("RunId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("RunId")
                        .IsUnique()
                        .HasFilter("[RunId] IS NOT NULL");

                    b.ToTable("Folders", "dbo");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("Name");

                    b.Property<int>("ProjectStatus")
                        .HasColumnType("int")
                        .HasColumnName("ProjectStatus");

                    b.HasKey("Id");

                    b.ToTable("Projects", "dbo");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Run", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("Name");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int")
                        .HasColumnName("ProjectId");

                    b.Property<int>("RootFolderId")
                        .HasColumnType("int")
                        .HasColumnName("RootFolderId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Runs", "dbo");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FolderId")
                        .HasColumnType("int")
                        .HasColumnName("FolderId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("Name");

                    b.Property<int>("RunId")
                        .HasColumnType("int")
                        .HasColumnName("RunId");

                    b.HasKey("Id");

                    b.HasIndex("FolderId");

                    b.ToTable("Tests", "dbo");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.TestResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ErrorMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("ErrorMessage");

                    b.Property<byte[]>("ScreenShot")
                        .IsRequired()
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("ScreenShot");

                    b.Property<string>("StackTrace")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("StackTrace");

                    b.Property<int>("TestId")
                        .HasColumnType("int")
                        .HasColumnName("TestId");

                    b.Property<int>("TestOutcome")
                        .HasColumnType("int")
                        .HasColumnName("TestOutcome");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("TestResults", "dbo");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.UserManagement.User", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Folder", b =>
                {
                    b.HasOne("ReportPortal.DAL.Models.RunProjectManagement.Folder", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.ClientCascade);

                    b.HasOne("ReportPortal.DAL.Models.RunProjectManagement.Run", "Run")
                        .WithOne("RootFolder")
                        .HasForeignKey("ReportPortal.DAL.Models.RunProjectManagement.Folder", "RunId")
                        .OnDelete(DeleteBehavior.ClientCascade);

                    b.Navigation("Parent");

                    b.Navigation("Run");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Run", b =>
                {
                    b.HasOne("ReportPortal.DAL.Models.RunProjectManagement.Project", "Project")
                        .WithMany("Runs")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Test", b =>
                {
                    b.HasOne("ReportPortal.DAL.Models.RunProjectManagement.Folder", "Folder")
                        .WithMany("Tests")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.TestResult", b =>
                {
                    b.HasOne("ReportPortal.DAL.Models.RunProjectManagement.Test", "Test")
                        .WithMany("TestResults")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Test");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Folder", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Tests");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Project", b =>
                {
                    b.Navigation("Runs");
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Run", b =>
                {
                    b.Navigation("RootFolder")
                        .IsRequired();
                });

            modelBuilder.Entity("ReportPortal.DAL.Models.RunProjectManagement.Test", b =>
                {
                    b.Navigation("TestResults");
                });
#pragma warning restore 612, 618
        }
    }
}
