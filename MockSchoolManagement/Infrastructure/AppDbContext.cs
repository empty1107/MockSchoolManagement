using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;

namespace MockSchoolManagement.Infrastructure
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class AppDbContext : DbContext
    {
        //DbContextOptions 实例负责承载应用中的配置信息，如连接字符串、数据库提供商等内容。
        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        {
        }
        //DbSet<TEntity>包含一个实体属性
        public DbSet<Student> Students { get; set; }

        //种子数据
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
        }
    }
}
