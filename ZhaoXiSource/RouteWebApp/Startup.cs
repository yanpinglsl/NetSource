using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RouteWebApp.RouteExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteWebApp
{
    /// <summary>
    /// �ܽ᣺
    ///     1���Զ���Լ��
    ///     2���Զ���·�ɹ���
    /// </summary>
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
            services.AddControllersWithViews();

            #region ·����չ
            services.AddDynamicRoute();

            //����Զ���·��Լ��
            services.AddRouting(options =>
            {
                options.ConstraintMap.Add("GenderConstraint", typeof(CustomGenderRouteConstraint));
            });
            #endregion
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            #region ��ȡ��·��ƥ����·����Ϣ
            //app.Use(next => context =>
            //{
            //    var endpoint = context.GetEndpoint();
            //    if (endpoint is null)
            //    {
            //        return next.Invoke(context);//û���оͼ���
            //        //return Task.CompletedTask;//û���о����κζ���
            //    }

            //    Console.WriteLine($"Endpoint: {endpoint.DisplayName}");

            //    if (endpoint is RouteEndpoint routeEndpoint)
            //    {
            //        Console.WriteLine(routeEndpoint.RoutePattern);
            //    }

            //    foreach (var metadata in endpoint.Metadata)
            //    {
            //        Console.WriteLine($"Endpoint has metadata: {metadata}");
            //    }
            //    return next.Invoke(context);
            //});
            #endregion

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});


            #region ·����չ
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "about-route",
                pattern: "about",
                defaults: new { controller = "Route", action = "About" }
                );//ָ��·��

                endpoints.UseMapGetConstraint();

                endpoints.MapControllerRoute(
                name: "range",
                pattern: "{controller=Home}/{action=Index}/{year:range(2019,2021)}-{month:range(1,12)}");

                ////α��̬
                //endpoints.MapControllerRoute(
                //    name: "static",
                //    pattern: "Item/{id:int}.html",
                //    defaults: new { controller = "Route", action = "PageInfo" });


                endpoints.MapControllerRoute(
                    name: "regular",
                    pattern: "{controller}/{action}/{year}-{month}",
                    constraints: new { year = "^\\d{4}$", month = "^\\d{2}$" },
                    defaults: new { controller = "Home", action = "Index", });

                endpoints.UseDynamicRouteDefault();


                ////endpoints.MapAreaControllerRoute(
                ////    name: "areas", "areas",
                ////    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                       name: "default",
                       pattern: "{controller=Home}/{action=Index}/{id?}");

                //MapGetָ������ʽ---MiniAPI
                endpoints.MapGet("/ElevenTest", async context =>
                {
                    await context.Response.WriteAsync($"This is ElevenTest");
                });
                //.RequireAuthorization();//Ҫ����Ȩ
                //.WithMetadata(new AuditPolicyAttribute());//·�����еĻ������Զ�Ӹ�����
            });
            #endregion
        }
    }
}
