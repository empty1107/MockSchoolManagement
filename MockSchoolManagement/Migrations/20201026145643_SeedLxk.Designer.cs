﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MockSchoolManagement.Infrastructure;

namespace MockSchoolManagement.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20201026145643_SeedLxk")]
    partial class SeedLxk
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MockSchoolManagement.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Major")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Students");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "648946942@qq.com",
                            Major = 2,
                            Name = "王鸿"
                        },
                        new
                        {
                            Id = 2,
                            Email = "474442945@qq.com",
                            Major = 3,
                            Name = "朱海"
                        },
                        new
                        {
                            Id = 3,
                            Email = "906067946@qq.com",
                            Major = 1,
                            Name = "林小康"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
