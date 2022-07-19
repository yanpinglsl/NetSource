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
            #region �ͻ���ģʽ��Ȩ
            ////ע��IdentityServer��������ע��һ�������ڴ�洢������ʱ״̬
            //services.AddIdentityServer(options =>
            //{
            //    //options.Events.RaiseErrorEvents = true;
            //    //options.Events.RaiseInformationEvents = true;
            //    //options.Events.RaiseFailureEvents = true;
            //    //options.Events.RaiseSuccessEvents = true;
            //})
            //    //����ģʽ�µ�ǩ��֤�飬�����������ü���
            //    .AddDeveloperSigningCredential()
            //    //OpenID Connect�����֤��Ϣ����
            //    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    //�����Դ����
            //    .AddInMemoryApiResources(Config.GetApis())//���ܱ�����Api��Դ��ӵ��ڴ���
            //                                              //���巶Χ
            //    .AddInMemoryApiScopes(Config.GetApiScopes())
            //    //�ͻ�����Ϣ����
            //    .AddInMemoryClients(Config.GetClients());//�ͻ���������ӵ��ڴ�
            #endregion
            #region ��Դ������������Ȩģʽ
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddTestUsers(Config.GetUsers());
            #endregion
            #region ��Ȩ��ģʽ��ʹ��is4ui��
            //services.AddControllersWithViews();
            ////ע��IDS4��Ϣ(��Ȩ��ģʽ)
            //services.AddIdentityServer()
            //         //����Tokenǩ����Ҫһ�Թ�Կ��˽Կ��������������ʹ�����AddDeveloperSigningCredential�����Զ�������Կ��
            //         //������������ʹ��֤�飬��ʱ��Ҫ����AddSigningCredential()
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

            //����IDS4
            app.UseIdentityServer();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    //endpoints.MapGet("/", async context =>
            //    //{
            //    //    await context.Response.WriteAsync("Hello World!");
            //    //});
            //    //�޸�Ĭ������·��
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}
