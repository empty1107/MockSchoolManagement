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
using Microsoft.AspNetCore.Identity;
using MockSchoolManagement.CustomerMiddlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MockSchoolManagement.Models;
using MockSchoolManagement.Security;

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
            services.AddControllersWithViews(config =>
            {
                //全局身份验证拦截验证，所以会自动跳转登录页
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                //config.EnableEndpointRouting = false;
            }).AddXmlSerializerFormatters();//AddXmlSerializerFormatters允许返回xml

            //注入服务的三种方法
            //AddSingleton 创建一个Singleton（单例）服务。首次请求创建，然后所有后续请求都会使用相同实例。
            //AddTransient 创建一个Transient（瞬时）服务，每次请求都会创建一个新的服务实例。
            //AddScoped 创建一个Scoped（作用域）服务。在范围内的每个请求中创建一个实例，但同一WEB请求中的其他服务在调用这个请求时，会使用相同的实例。注意：在一个客户端请求中是相同的，多个客户端请求中则不同。
            services.AddScoped<IStudentRepository, SQLStudentRepository>();

            //添加第三方登录，GitHub
            services.AddAuthentication().AddGitHub(options =>
            {
                options.ClientId = _configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = _configuration["Authentication:GitHub:ClientSecret"];
                options.Scope.Add("user:email");
            });

            //添加identity服务,AddIdentity()方法为系统提供默认的用户和角色类型的身份证验证系统
            //CustomIdentityErrorDescriber 自定义验证提示内容
            services.AddIdentity<ApplicationUser, IdentityRole>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>();

            //配置密码复杂度的验证，由 options.Password 提供
            //还有用户、登录、策略等配置信息，options 灵活配置
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;//密码中允许最大的重复字符数
                options.Password.RequireNonAlphanumeric = false;//是否需要非字母数字
                options.Password.RequireUppercase = false;//是否需要有大写字母
                options.Password.RequireLowercase = false;//是否需要有小写字母
                options.SignIn.RequireConfirmedEmail = true;//登录需要邮箱验证
            });

            //策略结合声明授权 RequireClaim（用于管理声明授权），RequireRole（用于管理角色授权），RequireAssertion（用于自定义授权）
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));//关联Delete Role声明
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));//管理Admin 角色
                //策略结合多个角色进行授权
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));
                options.AddPolicy("AllowedCountryPolicy", policy => policy.RequireClaim("Country", "China", "USA", "UK"));
                //自定义策略
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
            });

            //注入HttpContextAccessor，负责自定义策略，可看页面参数那些
            services.AddHttpContextAccessor();//帮我们获取http上下文
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                options.InvokeHandlersAfterFailure = false;//调用策略返回失败请求
            });
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            services.ConfigureApplicationCookie(options =>
            {
                //修改拒绝访问的路由地址
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                //修改登录地址的路由
                //options.LoginPath = new PathString("/Admin/Login");
                //修改注销地址的路由
                //options.LogoutPath = new PathString("/Admin/LogOut");
                //统一系统全局的cookie名称
                options.Cookie.Name = "MockSchoolCookieName";
                //登录用户cookie的有效期
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                //是否对cookie启用滑动过期时间
                options.SlidingExpiration = true;
            });
        }

        //授权访问，拥有 or 关系的策略就需要用到 RequireAssertion，否则全是 and 关系可以直接使用流水线策略
        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") &&
                   context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                   context.User.IsInRole("Super Admin");
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
            //添加验证中间件，身份证验证中间件
            app.UseAuthentication();
            //路由中间件
            app.UseRouting();
            //用户授权中间件，必须放于 UseRouting 和 UseEndpoints之间
            app.UseAuthorization();
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
