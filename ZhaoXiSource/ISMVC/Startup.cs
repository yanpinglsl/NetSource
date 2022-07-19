using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ISMVC
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
            services.AddControllersWithViews();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";//设置验证时使用的默认方案
                    options.DefaultChallengeScheme = "oidc";//默认方案验证失败后的确认验证结果方案
                                                            //options.DefaultForbidScheme = "Cookie";//设置禁止访问时使用的默认方案
                                                            //options.DefaultSignInScheme = "Cookie"; //设置登录的默认方案。
                                                            //options.DefaultSignOutScheme = "Cookie";//设置退出的默认方案。
                })
                .AddCookie("Cookies")
                //添加OpenIdConnect认证方案
                //需要nuget Microsoft.AspNetCore.Authentication.OpenIdConnect
                .AddOpenIdConnect("oidc", options =>
                {
                        //远程认证地址
                        options.Authority = "https://localhost:5000";
                        //Https强制要求标识
                        options.RequireHttpsMetadata = true;

                        //客户端ID
                        options.ClientId = "mvc";    //客户端ID
                        options.ClientSecret = "123456"; //客户端秘钥
                                                         //授权码模式
                        options.ResponseType = OpenIdConnectResponseType.Code;
                    options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.SaveTokens = true;
                });
                // 配置cookie策略
                //services.AddNonBreakingSameSiteCookies();
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

            //app.UseCookiePolicy();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            //开启认证
            app.UseAuthentication();
            //开启授权
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //修改默认路由， RequireAuthorization
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            });
        }
    }
}
