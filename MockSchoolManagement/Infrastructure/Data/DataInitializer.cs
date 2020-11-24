using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Data
{
    /// <summary>
    /// 数据初始化
    /// </summary>
    public static class DataInitializer
    {
        /// <summary>
        /// 用户数据初始化
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            //IServiceScope 以解析应用范围内配置的服务，此方法可以用于在启动时访问有作用域的服务，以便运行初始化任务。
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                #region 学生种子信息
                if (dbcontext.Students.Any())
                {
                    return builder;//数据已经初始化
                }
                var students = new List<Student>
                {
                    new Student{ Name="徐维维",Email="xuweiwei@qq.com",Major=Models.EnumTypes.MajorEnum.Mathematics, EnrollmentDate=DateTime.Parse("1991-08-20")},
                    new Student{ Name="朱海",Email="zhuhai@qq.com",Major=Models.EnumTypes.MajorEnum.None, EnrollmentDate=DateTime.Parse("1991-03-12")},
                    new Student{ Name="林小康",Email="linxiaokang@qq.com",Major=Models.EnumTypes.MajorEnum.ComputerScience, EnrollmentDate=DateTime.Parse("1992-10-09")},
                };
                foreach (var stu in students)
                {
                    dbcontext.Students.Add(stu);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 课程种子数据
                if (dbcontext.Courses.Any())
                {
                    return builder;
                }
                var courses = new[] {
                    new Course{ CourseID=1050,Title="数学",Credits=3},
                    new Course{ CourseID=4022,Title="政治",Credits=3},
                    new Course{ CourseID=4041,Title="物理",Credits=3},
                    new Course{ CourseID=1045,Title="化学",Credits=4},
                    new Course{ CourseID=3141,Title="生物",Credits=4},
                    new Course{ CourseID=2021,Title="英语",Credits=3},
                    new Course{ CourseID=2042,Title="历史",Credits=4},
                };
                foreach (var course in courses)
                {
                    dbcontext.Courses.Add(course);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 学生课程关联种子数据
                var studentCourses = new[] {
                    new StudentCourse{CourseID = 1050,StudentID = 1010},
                    new StudentCourse{CourseID = 4022,StudentID = 1010},
                    new StudentCourse{CourseID = 2021,StudentID = 1010},
                    new StudentCourse{CourseID = 4022,StudentID = 1011},
                    new StudentCourse{CourseID = 2021,StudentID = 1012}
                };
                foreach (var  sc in studentCourses)
                {
                    dbcontext.StudentCourses.Add(sc);
                }
                dbcontext.SaveChanges();
                #endregion

                #region 用户种子数据
                if (dbcontext.Users.Any())
                {
                    return builder;
                }
                var user = new ApplicationUser
                {
                    Email = "648946942@qq.com",
                    UserName = "648946942@qq.com",
                    EmailConfirmed = true,
                    City = "重庆"
                };
                userManager.CreateAsync(user, "123456").Wait();//异步等待
                dbcontext.SaveChanges();
                var adminRole = "Admin";

                var role = new IdentityRole { Name = adminRole };

                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();

                dbcontext.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
                dbcontext.SaveChanges();
                #endregion
            }
            return builder;
        }
    }
}
