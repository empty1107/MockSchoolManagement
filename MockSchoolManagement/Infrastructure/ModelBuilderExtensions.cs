using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure
{
    /// <summary>
    /// 为了让AppDbContext类种子数据更为“干净”
    /// </summary>
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //指定实体在数据库中生成的名称
            modelBuilder.Entity<Course>().ToTable("Course", "School");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse", "School");
            modelBuilder.Entity<Student>().ToTable("Student", "School");
            //联合主键
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.TeacherID });

        }
    }
}
