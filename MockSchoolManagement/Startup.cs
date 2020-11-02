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
            services.AddControllersWithViews(a => a.EnableEndpointRouting = false).AddXmlSerializerFormatters();//AddXmlSerializerFormatters������xml

            //ע���������ַ���
            //AddSingleton ����һ��Singleton�������������״����󴴽���Ȼ�����к������󶼻�ʹ����ͬʵ����
            //AddTransient ����һ��Transient��˲ʱ������ÿ�����󶼻ᴴ��һ���µķ���ʵ����
            //AddScoped ����һ��Scoped�������򣩷����ڷ�Χ�ڵ�ÿ�������д���һ��ʵ������ͬһWEB�����е����������ڵ����������ʱ����ʹ����ͬ��ʵ����ע�⣺��һ���ͻ�������������ͬ�ģ�����ͻ�����������ͬ��
            services.AddScoped<IStudentRepository, SQLStudentRepository>();

            //���identity����,AddIdentity()����Ϊϵͳ�ṩĬ�ϵ��û��ͽ�ɫ���͵����֤��֤ϵͳ
            //CustomIdentityErrorDescriber �Զ�����֤��ʾ����
            services.AddIdentity<IdentityUser, IdentityRole>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddEntityFrameworkStores<AppDbContext>();

            //�������븴�Ӷȵ���֤���� options.Password �ṩ
            //�����û�����¼�����Ե�������Ϣ��options �������
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;//���������������ظ��ַ���
                options.Password.RequireNonAlphanumeric = false;//�Ƿ���Ҫ����ĸ����
                options.Password.RequireUppercase = false;//�Ƿ���Ҫ�д�д��ĸ
                options.Password.RequireLowercase = false;//�Ƿ���Ҫ��Сд��ĸ
            });
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
            //�����֤�м��
            app.UseAuthentication();
            //·���м��
            app.UseRouting();
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
