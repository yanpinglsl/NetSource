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
            services.AddScoped<SwaggerGenerator>(); //ע��SwaggerGenerator,�������ֱ��ʹ���������
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Version = "V1",   //�汾 
                    Title = $"XUnit.Core �ӿ��ĵ�-NetCore3.1",  //����
                    Description = $"XUnit.Core Http API v1",    //����
                    Contact = new OpenApiContact { Name = "����Ԫ", Email = "", Url = new Uri("http://i3yuan.cnblogs.com") },  
                    License = new OpenApiLicense { Name = "����Ԫ���֤", Url = new Uri("http://i3yuan.cnblogs.com") }
                });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//��ȡӦ�ó�������Ŀ¼�����ԣ����ܹ���Ŀ¼Ӱ�죬������ô˷�����ȡ·����
               //var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "XUnit.Core.xml");//������Ǹո����õ�xml�ļ���
               // c.IncludeXmlComments(xmlPath);//Ĭ�ϵĵڶ���������false,�Է�����ע��
                 c.IncludeXmlComments(xmlPath,true); // �����controller��ע��



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

                #region Swagger ֻ�ڿ���������ʹ��
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"XUnit.Core V1");
                    c.RoutePrefix = string.Empty;     //�����Ϊ�� ����·����Ϊ ������/index.html,ע��localhost:8001/swagger�Ƿ��ʲ�����
                                                      //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�
                                                      // c.RoutePrefix = "swagger"; // ������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "swagger"; �����·��Ϊ ������/swagger/index.html

                    c.DocumentTitle = "XUnit.Core �����ĵ�����";
                    #region �Զ�����ʽ

                    //css ע��
                    c.InjectStylesheet("/css/swaggerdoc.css");
                    c.InjectStylesheet("/css/app.min.css");
                    //js ע��
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
