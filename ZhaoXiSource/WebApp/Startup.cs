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
            services.AddSession();//如果没有该配置，则不支持Session
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation(); //视图即时编译,也可在创建项目时勾选Enable Razor runtime complation选项
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
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))  //执行文件下的wwwroot文件夹
            }); //可以在这里配置css路径

            app.UseSession();//把Sesssion处理装配到框架中来
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

        //一、替换IOC容器：
        // 1.Nuget引入程序集Autofac+Autofac.Extensions.DependencyInjection
        // 2.既然要使用Autofac--的告诉.ASP.NET Core5.0 框架要使用Autofac
        //   .UseServiceProviderFactory(new AutofacServiceProviderFactory()) ---Program.cs
        // 3.在Startup增加一个配置Autofac注册服务的方法


        //二、Autofac支持属性+方法注入
        //1.一个类中会有很多个属性---可以是当前类自己的---也有可能是来自于父类的--难道每一个属性都给支持注入？----应该是有需要就注入---应该让我们自己来控制，需要注入就注入---给一个标记--就注入--标记？---特性
        //2.定义一个特性---指定智能标记在属性上
        //3.定义一个CustomPropertySelector 选择哪个属性需要依赖注入
        //4.在注册服务的时候，指定要支持属性注入.PropertiesAutowired(new CustomPropertySelector());


        //方法注入：
        //1.在实例构建完了以后，把指定的方法给执行一遍；
        //2.实现：.OnActivated(e => e.Instance.SetService(e.Context.Resolve<ISysUserService>()))

        /// <summary>
        /// Autofac注册服务的地方---最后Autofac 容器中包含的：是包含了内容容器中注册服务+在这里注入的服务
        /// </summary>
        /// <param name="containerBuilder"></param>
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            ////如果要注册某一个Dll中的服务；可以通过反射把dll加载进来；然后再读取出来抽象和具体；然后逐个注册进去；

            //注册服务
            containerBuilder.RegisterType<CompanyService>()
                .OnActivated(e => e.Instance.SetService(e.Context.Resolve<ISysUserService>())) //方法注入--再获取实例后，把指定的某一个方法给执行以下
                .As<ICompanyService>()
                .PropertiesAutowired(new CustomPropertySelector());  //Autoafc自己来调用的这个方法


            containerBuilder.RegisterType<SysUserService>().As<ISysUserService>();
        }
    }
}
