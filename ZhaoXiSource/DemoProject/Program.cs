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

        #region 配置文件
             .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
             {
                 #region 内存Provider
                 var memoryConfig = new Dictionary<string, string>
                 {
                    {"TodayMemory", "0624-Memory"},
                    {"RabbitMQOptions:HostName", "192.168.3.254-Memory"},
                     {"RabbitMQOptions:UserName", "guest-Memory"},
                     {"RabbitMQOptions:Password", "guest-Memory"}
                 };
                 configurationBuilder.AddInMemoryCollection(memoryConfig);
                 #endregion

                 #region Apollo分布式配置中心
                 //LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
                 //configurationBuilder
                 //    .AddApollo(configurationBuilder.Build().GetSection("apollo"))
                 //    .AddDefault()
                 //    .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//自定义的private NameSpace
                 //    .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo中默认NameSpace的名称
                 #endregion

                 #region XML
                 //configurationBuilder.AddXmlFile("appsettings.xml", optional: false, reloadOnChange: true);
                 #endregion

                 #region 自定义Provider
                 configurationBuilder.AddCustomConfiguration(option=> {
                     option.LogTag = "This is CustomConfiguration";
                     option.DataChangeAction = null;
                     option.DataInitFunc = null;//未提供
                 });
                 #endregion
             })
        #endregion

        #region 扩展日志
                //.ConfigureLogging((context, loggingBuilder) =>
                //{
                //    loggingBuilder.ClearProviders();//清理其他
                //                                    //loggingBuilder.AddConsole()
                //                                    //                .AddDebug()
                //                                    //                ;


                //    #region log4net
                //    //loggingBuilder.AddFilter("System", LogLevel.Warning);//过滤掉命名空间
                //    //loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//
                //    //loggingBuilder.AddLog4Net();//路径：默认为log4net.config
                //    #endregion
                //})
        #endregion

        #region IOC容器
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        #endregion


        #region ConfigureServices
                ////这时候还没有IOC，是放入一个委托
                //.ConfigureServices((context, services) =>
                //{
                //    services.AddTransient<IStartupFilter, CustomStartupFilter>();
                //})//跟直接Startup的ConfigureServices效果完全一样
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
                    //    // });//没有本地证书
                    //    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                    //    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                    //});
                    ////webBuilder.UseUrls("http://localhost:5003", "https://localhost:5004");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
