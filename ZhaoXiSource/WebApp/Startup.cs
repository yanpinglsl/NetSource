using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Practice.Interface;
using Practice.Interface.AutofacExtension;
using Practice.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();//���û�и����ã���֧��Session
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation(); //��ͼ��ʱ����,Ҳ���ڴ�����Ŀʱ��ѡEnable Razor runtime complationѡ��
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))  //ִ���ļ��µ�wwwroot�ļ���
            }); //��������������css·��

            app.UseSession();//��Sesssion����װ�䵽�������
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=First}/{action=Index}/{id?}");
            });
        }

        //һ���滻IOC������
        // 1.Nuget�������Autofac+Autofac.Extensions.DependencyInjection
        // 2.��ȻҪʹ��Autofac--�ĸ���.ASP.NET Core5.0 ���Ҫʹ��Autofac
        //   .UseServiceProviderFactory(new AutofacServiceProviderFactory()) ---Program.cs
        // 3.��Startup����һ������Autofacע�����ķ���


        //����Autofac֧������+����ע��
        //1.һ�����л��кܶ������---�����ǵ�ǰ���Լ���---Ҳ�п����������ڸ����--�ѵ�ÿһ�����Զ���֧��ע�룿----Ӧ��������Ҫ��ע��---Ӧ���������Լ������ƣ���Ҫע���ע��---��һ�����--��ע��--��ǣ�---����
        //2.����һ������---ָ�����ܱ����������
        //3.����һ��CustomPropertySelector ѡ���ĸ�������Ҫ����ע��
        //4.��ע������ʱ��ָ��Ҫ֧������ע��.PropertiesAutowired(new CustomPropertySelector());


        //����ע�룺
        //1.��ʵ�����������Ժ󣬰�ָ���ķ�����ִ��һ�飻
        //2.ʵ�֣�.OnActivated(e => e.Instance.SetService(e.Context.Resolve<ISysUserService>()))

        /// <summary>
        /// Autofacע�����ĵط�---���Autofac �����а����ģ��ǰ���������������ע�����+������ע��ķ���
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            ////���Ҫע��ĳһ��Dll�еķ��񣻿���ͨ�������dll���ؽ�����Ȼ���ٶ�ȡ��������;��壻Ȼ�����ע���ȥ��

            //ע�����
            containerBuilder.RegisterType<CompanyService>()
                .OnActivated(e => e.Instance.SetService(e.Context.Resolve<ISysUserService>())) //����ע��--�ٻ�ȡʵ���󣬰�ָ����ĳһ��������ִ������
                .As<ICompanyService>()
                .PropertiesAutowired(new CustomPropertySelector());  //Autoafc�Լ������õ��������


            containerBuilder.RegisterType<SysUserService>().As<ISysUserService>();
        }
    }
}
