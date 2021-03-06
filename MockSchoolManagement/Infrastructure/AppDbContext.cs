﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MockSchoolManagement.Infrastructure
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        //DbContextOptions 实例负责承载应用中的配置信息，如连接字符串、数据库提供商等内容。
        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        {
        }
        //DbSet<TEntity>包含一个实体属性
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //添加base是因为要封装Seed()方法，所以重写OnModelCreating()方法。出现这个错误是因为我们在DbContext类中重写了OnModelCreating()方法，
            //但未调用基本IdentityDbContext类OnModelCreating()方法，我们需要调用基类OnModelCreating()使用该方法的基础关键字
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();//种子数据

            //获取当前系统中所有领域模型上的外键列表
            var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var foreignKey in foreignKeys)
            {
                //将它们的删除行为配置为 Restrict ，即无操作
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
