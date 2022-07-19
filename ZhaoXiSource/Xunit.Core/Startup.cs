using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using XUnit.Core.Helper;

namespace XUnit.Core
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
            services.AddScoped<SwaggerGenerator>(); //注入SwaggerGenerator,后面可以直接使用这个方法
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",   //版本 
                    Title = $"XUnit.Core 接口文档-NetCore3.1",  //标题
                    Description = $"XUnit.Core Http API v1",    //描述
                    Contact = new OpenApiContact { Name = "艾三元", Email = "", Url = new Uri("http://i3yuan.cnblogs.com") },  
                    License = new OpenApiLicense { Name = "艾三元许可证", Url = new Uri("http://i3yuan.cnblogs.com") }
                });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
               //var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "XUnit.Core.xml");//这个就是刚刚配置的xml文件名
               // c.IncludeXmlComments(xmlPath);//默认的第二个参数是false,对方法的注释
                 c.IncludeXmlComments(xmlPath,true); // 这个是controller的注释



            });
            services.AddScoped<SpireDocHelper>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region Swagger 只在开发环节中使用
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"XUnit.Core V1");
                    c.RoutePrefix = string.Empty;     //如果是为空 访问路径就为 根域名/index.html,注意localhost:8001/swagger是访问不到的
                                                      //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件
                                                      // c.RoutePrefix = "swagger"; // 如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html

                    c.DocumentTitle = "XUnit.Core 在线文档调试";
                    #region 自定义样式

                    //css 注入
                    c.InjectStylesheet("/css/swaggerdoc.css");
                    c.InjectStylesheet("/css/app.min.css");
                    //js 注入
                    c.InjectJavascript("/js/jquery.js");
                    c.InjectJavascript("/js/swaggerdoc.js");
                    c.InjectJavascript("/js/app.min.js");
                   
                    #endregion

                });
                #endregion

            }
            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
