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
                options.DefaultScheme = "Cookies";//������֤ʱʹ�õ�Ĭ�Ϸ���
                    options.DefaultChallengeScheme = "oidc";//Ĭ�Ϸ�����֤ʧ�ܺ��ȷ����֤�������
                                                            //options.DefaultForbidScheme = "Cookie";//���ý�ֹ����ʱʹ�õ�Ĭ�Ϸ���
                                                            //options.DefaultSignInScheme = "Cookie"; //���õ�¼��Ĭ�Ϸ�����
                                                            //options.DefaultSignOutScheme = "Cookie";//�����˳���Ĭ�Ϸ�����
                })
                .AddCookie("Cookies")
                //���OpenIdConnect��֤����
                //��Ҫnuget Microsoft.AspNetCore.Authentication.OpenIdConnect
                .AddOpenIdConnect("oidc", options =>
                {
                        //Զ����֤��ַ
                        options.Authority = "https://localhost:5000";
                        //Httpsǿ��Ҫ���ʶ
                        options.RequireHttpsMetadata = true;

                        //�ͻ���ID
                        options.ClientId = "mvc";    //�ͻ���ID
                        options.ClientSecret = "123456"; //�ͻ�����Կ
                                                         //��Ȩ��ģʽ
                        options.ResponseType = OpenIdConnectResponseType.Code;
                    options.ResponseMode = OpenIdConnectResponseMode.Query;
                    options.SaveTokens = true;
                });
                // ����cookie����
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

            //������֤
            app.UseAuthentication();
            //������Ȩ
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //�޸�Ĭ��·�ɣ� RequireAuthorization
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            });
        }
    }
}
