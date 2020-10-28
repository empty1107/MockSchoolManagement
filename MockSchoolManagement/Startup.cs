using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure;

namespace MockSchoolManagement
{
    /// <summary>
    /// 程序启动类
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        // 配置应用程序所需要的服务
        public void ConfigureServices(IServiceCollection services)
        {
            //使用SqlServer数据库，通过IConfiguration访问去获取，自定义名称的Mock
            //StudentDBConnection 作为我们的连接字符串，AddDbContextPool 提供了数据连接池，如果连接池可用就不会创建新的实例。
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));

            //注册配置文件服务
            services.AddControllersWithViews(a => a.EnableEndpointRouting = false).AddXmlSerializerFormatters();//AddXmlSerializerFormatters允许返回xml

            //注入服务的三种方法
            //AddSingleton 创建一个Singleton（单例）服务。首次请求创建，然后所有后续请求都会使用相同实例。
            //AddTransient 创建一个Transient（瞬时）服务，每次请求都会创建一个新的服务实例。
            //AddScoped 创建一个Scoped（作用域）服务。在范围内的每个请求中创建一个实例，但同一WEB请求中的其他服务在调用这个请求时，会使用相同的实例。注意：在一个客户端请求中是相同的，多个客户端请求中则不同。
            services.AddScoped<IStudentRepository, SQLStudentRepository>();
        }

        /// <summary>
        /// 方法配置应用程序的请求处理管道
        /// 在ASP.NET Core中，中间件是一个可以处理HTTP请求或响应的软件管道
        /// 中间件是按照添加到管道的顺序执行的。
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())//开发环境
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))//非开发环境，显示统一错误页面
            {
                app.UseExceptionHandler("/Error");
                //app.UseStatusCodePages();显示默认页面，仅文本

                //需要拦截访问失败的http状态码，返回自定义错误视图
                //占位符 ｛0｝ ，它会自动接收HTTP中的状态码
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");//重定向到统一错误页面，页面地址显示为Error，会改变请求地址

                //建议使用，可以保留错误的请求信息，便于日志分析
                app.UseStatusCodePagesWithReExecute("/Error/{0}");//不会重定向，页面地址显示为错误的链接地址
            }
            //使用纯静态文件支持的中间件，而不使用带有终端的中间件
            app.UseStaticFiles();
            //路由中间件
            app.UseRouting();
            //终结点路由
            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
