using JWT.AuthenticationCenter.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWT.AuthenticationCenter
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT.AuthenticationCenter", Version = "v1" });
            });
            #region HS256 对称可逆加密
            services.AddScoped<IJWTService, JWTHSService>();
            services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTTokenOptions"));
            #endregion

            #region RS256 非对称可逆加密，需要获取一次公钥
            //string keyDir = Directory.GetCurrentDirectory();
            //if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            //{
            //    keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            //}

            //services.AddScoped<IJWTService, JWTRSService>();
            //services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTTokenOptions"));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT.AuthenticationCenter v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
