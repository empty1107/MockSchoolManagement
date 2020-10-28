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
            modelBuilder.Entity<Student>().HasData(
              new Student
              {
                  Id = 1,
                  Name = "王鸿",
                  Major = Models.EnumTypes.MajorEnum.ElectronicCommerce,
                  Email = "648946942@qq.com"
              });
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 2,
                    Name = "朱海",
                    Major = Models.EnumTypes.MajorEnum.Mathematics,
                    Email = "474442945@qq.com"
                });
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 3,
                    Name = "林小康",
                    Major = Models.EnumTypes.MajorEnum.ComputerScience,
                    Email = "906067946@qq.com"
                });
        }
    }
}
