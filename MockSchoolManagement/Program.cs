using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace MockSchoolManagement
{
    public class Program
    {
        /// <summary>
        /// ������ڣ�asp.net core Ӧ�ó��������Ϊ����̨Ӧ�ó�������
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) //CreateDefaultBuilder ������Ϊ�ڷ������ϴ����������õ�Ĭ��ֵ������
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                //logging.AddConsole();
                //logging.AddDebug();
                //logging.AddEventSourceLogger();
                //����NLog��Ϊ��־�ṩ����֮һ
                logging.AddNLog();
            })
                .ConfigureWebHostDefaults(webBuilder => 
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
