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
    /// ����������
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        // ����Ӧ�ó�������Ҫ�ķ���
        public void ConfigureServices(IServiceCollection services)
        {
            //ʹ��SqlServer���ݿ⣬ͨ��IConfiguration����ȥ��ȡ���Զ������Ƶ�Mock
            //StudentDBConnection ��Ϊ���ǵ������ַ�����AddDbContextPool �ṩ���������ӳأ�������ӳؿ��þͲ��ᴴ���µ�ʵ����
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MockStudentDBConnection")));

            //ע�������ļ�����
            services.AddControllersWithViews(config =>
            {
                //ȫ�������֤������֤�����Ի��Զ���ת��¼ҳ
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                //config.EnableEndpointRouting = false;
            }).AddXmlSerializerFormatters();//AddXmlSerializerFormatters������xml

            //ע���������ַ���
            //AddSingleton ����һ��Singleton�������������״����󴴽���Ȼ�����к������󶼻�ʹ����ͬʵ����
            //AddTransient ����һ��Transient��˲ʱ������ÿ�����󶼻ᴴ��һ���µķ���ʵ����
            //AddScoped ����һ��Scoped�������򣩷����ڷ�Χ�ڵ�ÿ�������д���һ��ʵ������ͬһWEB�����е����������ڵ����������ʱ����ʹ����ͬ��ʵ����ע�⣺��һ���ͻ�������������ͬ�ģ�����ͻ�����������ͬ��
            services.AddScoped<IStudentRepository, SQLStudentRepository>();

            //��ӵ�������¼��GitHub
            services.AddAuthentication().AddGitHub(options =>
            {
                options.ClientId = _configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = _configuration["Authentication:GitHub:ClientSecret"];
                options.Scope.Add("user:email");
            });

            //���identity����,AddIdentity()����Ϊϵͳ�ṩĬ�ϵ��û��ͽ�ɫ���͵����֤��֤ϵͳ
            //CustomIdentityErrorDescriber �Զ�����֤��ʾ����
            services.AddIdentity<ApplicationUser, IdentityRole>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>();

            //�������븴�Ӷȵ���֤���� options.Password �ṩ
            //�����û�����¼�����Ե�������Ϣ��options �������
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;//���������������ظ��ַ���
                options.Password.RequireNonAlphanumeric = false;//�Ƿ���Ҫ����ĸ����
                options.Password.RequireUppercase = false;//�Ƿ���Ҫ�д�д��ĸ
                options.Password.RequireLowercase = false;//�Ƿ���Ҫ��Сд��ĸ
                options.SignIn.RequireConfirmedEmail = true;//��¼��Ҫ������֤
            });

            //���Խ��������Ȩ RequireClaim�����ڹ���������Ȩ����RequireRole�����ڹ����ɫ��Ȩ����RequireAssertion�������Զ�����Ȩ��
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));//����Delete Role����
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));//����Admin ��ɫ
                //���Խ�϶����ɫ������Ȩ
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));
                options.AddPolicy("AllowedCountryPolicy", policy => policy.RequireClaim("Country", "China", "USA", "UK"));
                //�Զ������
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context => AuthorizeAccess(context)));
            });

            //ע��HttpContextAccessor�������Զ�����ԣ��ɿ�ҳ�������Щ
            services.AddHttpContextAccessor();//�����ǻ�ȡhttp������
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
                options.InvokeHandlersAfterFailure = false;//���ò��Է���ʧ������
            });
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            services.ConfigureApplicationCookie(options =>
            {
                //�޸ľܾ����ʵ�·�ɵ�ַ
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
                //�޸ĵ�¼��ַ��·��
                //options.LoginPath = new PathString("/Admin/Login");
                //�޸�ע����ַ��·��
                //options.LogoutPath = new PathString("/Admin/LogOut");
                //ͳһϵͳȫ�ֵ�cookie����
                options.Cookie.Name = "MockSchoolCookieName";
                //��¼�û�cookie����Ч��
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                //�Ƿ��cookie���û�������ʱ��
                options.SlidingExpiration = true;
            });
        }

        //��Ȩ���ʣ�ӵ�� or ��ϵ�Ĳ��Ծ���Ҫ�õ� RequireAssertion������ȫ�� and ��ϵ����ֱ��ʹ����ˮ�߲���
        private bool AuthorizeAccess(AuthorizationHandlerContext context)
        {
            return context.User.IsInRole("Admin") &&
                   context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                   context.User.IsInRole("Super Admin");
        }

        /// <summary>
        /// ��������Ӧ�ó����������ܵ�
        /// ��ASP.NET Core�У��м����һ�����Դ���HTTP�������Ӧ������ܵ�
        /// �м���ǰ�����ӵ��ܵ���˳��ִ�еġ�
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())//��������
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))//�ǿ�����������ʾͳһ����ҳ��
            {
                app.UseExceptionHandler("/Error");
                //app.UseStatusCodePages();��ʾĬ��ҳ�棬���ı�

                //��Ҫ���ط���ʧ�ܵ�http״̬�룬�����Զ��������ͼ
                //ռλ�� ��0�� �������Զ�����HTTP�е�״̬��
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");//�ض���ͳһ����ҳ�棬ҳ���ַ��ʾΪError����ı������ַ

                //����ʹ�ã����Ա��������������Ϣ��������־����
                app.UseStatusCodePagesWithReExecute("/Error/{0}");//�����ض���ҳ���ַ��ʾΪ��������ӵ�ַ
            }
            //ʹ�ô���̬�ļ�֧�ֵ��м��������ʹ�ô����ն˵��м��
            app.UseStaticFiles();
            //�����֤�м�������֤��֤�м��
            app.UseAuthentication();
            //·���м��
            app.UseRouting();
            //�û���Ȩ�м����������� UseRouting �� UseEndpoints֮��
            app.UseAuthorization();
            //�ս��·��
            app.UseEndpoints((endpoints) =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
