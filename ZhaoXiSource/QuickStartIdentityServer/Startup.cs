using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickStartIdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region 客户端模式授权
            ////注册IdentityServer，它还会注册一个基于内存存储的运行时状态
            //services.AddIdentityServer(options =>
            //{
            //    //options.Events.RaiseErrorEvents = true;
            //    //options.Events.RaiseInformationEvents = true;
            //    //options.Events.RaiseFailureEvents = true;
            //    //options.Events.RaiseSuccessEvents = true;
            //})
            //    //开发模式下的签名证书，开发环境启用即可
            //    .AddDeveloperSigningCredential()
            //    //OpenID Connect相关认证信息配置
            //    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    //相关资源配置
            //    .AddInMemoryApiResources(Config.GetApis())//把受保护的Api资源添加到内存中
            //                                              //定义范围
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    //客户端信息配置
            //    .AddInMemoryClients(Config.GetClients());//客户端配置添加到内存
            #endregion
            #region 资源所有者密码授权模式
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddTestUsers(Config.GetUsers());
            #endregion
            #region 授权码模式（使用is4ui）
            //services.AddControllersWithViews();
            ////注册IDS4信息(授权码模式)
            //services.AddIdentityServer()
            //         //对于Token签名需要一对公钥和私钥，开发环境可以使用这个AddDeveloperSigningCredential，会自动生成密钥。
            //         //生产环境还是使用证书，此时需要调用AddSigningCredential()
            //         .AddDeveloperSigningCredential()
            //         .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //         .AddInMemoryApiResources(Config.GetApis())
            //         .AddInMemoryClients(Config.GetClients())
            //         .AddTestUsers(Config.GetUsers());
            #endregion

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();

            //启用IDS4
            app.UseIdentityServer();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    //endpoints.MapGet("/", async context =>
            //    //{
            //    //    await context.Response.WriteAsync("Hello World!");
            //    //});
            //    //修改默认启动路由
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}
