using Autofac.Extensions.DependencyInjection;
using ExtendLib.LogExtend;
using ExtendLib.StartupExtend;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DemoProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

        #region �����ļ�
             .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
             {
                 #region �ڴ�Provider
                 var memoryConfig = new Dictionary<string, string>
                 {
                    {"TodayMemory", "0624-Memory"},
                    {"RabbitMQOptions:HostName", "192.168.3.254-Memory"},
                     {"RabbitMQOptions:UserName", "guest-Memory"},
                     {"RabbitMQOptions:Password", "guest-Memory"}
                 };
                 configurationBuilder.AddInMemoryCollection(memoryConfig);
                 #endregion

                 #region Apollo�ֲ�ʽ��������
                 //LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
                 //configurationBuilder
                 //    .AddApollo(configurationBuilder.Build().GetSection("apollo"))
                 //    .AddDefault()
                 //    .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//�Զ����private NameSpace
                 //    .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo��Ĭ��NameSpace������
                 #endregion

                 #region XML
                 //configurationBuilder.AddXmlFile("appsettings.xml", optional: false, reloadOnChange: true);
                 #endregion

                 #region �Զ���Provider
                 configurationBuilder.AddCustomConfiguration(option=> {
                     option.LogTag = "This is CustomConfiguration";
                     option.DataChangeAction = null;
                     option.DataInitFunc = null;//δ�ṩ
                 });
                 #endregion
             })
        #endregion

        #region ��չ��־
                //.ConfigureLogging((context, loggingBuilder) =>
                //{
                //    loggingBuilder.ClearProviders();//��������
                //                                    //loggingBuilder.AddConsole()
                //                                    //                .AddDebug()
                //                                    //                ;


                //    #region log4net
                //    //loggingBuilder.AddFilter("System", LogLevel.Warning);//���˵������ռ�
                //    //loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//
                //    //loggingBuilder.AddLog4Net();//·����Ĭ��Ϊlog4net.config
                //    #endregion
                //})
        #endregion

        #region IOC����
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        #endregion


        #region ConfigureServices
                ////��ʱ��û��IOC���Ƿ���һ��ί��
                //.ConfigureServices((context, services) =>
                //{
                //    services.AddTransient<IStartupFilter, CustomStartupFilter>();
                //})//��ֱ��Startup��ConfigureServicesЧ����ȫһ��
        #endregion

            .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.ConfigureKestrel(serverOptions =>
                    //{
                    //    serverOptions.Limits.MaxConcurrentConnections = 100;
                    //    serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
                    //    serverOptions.Limits.MaxRequestBodySize = 10 * 1024;
                    //    serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    //    serverOptions.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));

                    //    serverOptions.Listen(IPAddress.Loopback, 8000);
                    //    serverOptions.Listen(IPAddress.Loopback, 9000);

                    //    //serverOptions.Listen(IPAddress.Loopback, 9099, o => o.Protocols =
                    //    //     HttpProtocols.Http2);
                    //    //serverOptions.Listen(IPAddress.Loopback, 5001, listenOptions =>
                    //    // {
                    //    //     listenOptions.UseHttps("testCert.pfx", "testPassword");
                    //    // });//û�б���֤��
                    //    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                    //    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                    //});
                    ////webBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
