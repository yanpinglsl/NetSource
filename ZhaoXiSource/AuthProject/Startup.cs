using AuthenticationExtend;
using AuthProject.CustomAuth;
using AuthProject.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthProject
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

            #region ��Ȩ��Ȩ  ͬʱ��Чֻ��һ��
            #region ������Ȩ����---����
            ////services.AddAuthentication();//û���κ�Scheme���У�����Ҫ����DefaultScheme
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//��Ҫ��Ȩ���ұ���ָ��Ĭ�Ϸ���
            //         .AddCookie();//ʹ��Cookie�ķ�ʽ
            ////services.CustomAddAuthenticationCore();//�滻��IOC,������Ժ���չ
            #endregion

            #region �Զ����ȨHandler
            ////ʹ��Url���������û���Ϣ
            //services.AddAuthentication(options =>
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");

            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //});//����Ĭ��ע��,�Զ������Handler

            ////services.AddAuthentication(UrlTokenAuthenticationDefaults.AuthenticationScheme)
            ////.AddCookie();

            //////services.CustomAddAuthenticationCore();//�滻��IOC,������Ժ���չ

            #endregion

            #region �Զ����Ȩhandler �ѵ�Cookie--��handler
            //services.AddAuthentication(options =>//����Ĭ��ע��,�Զ������Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme); //�����ø�Cookie����

            //////services.CustomAddAuthenticationCore();//�滻��IOC,������Ժ���չ
            #endregion

            #region �Զ����Ȩhandler �ѵ�Cookie--��handler---������Ȩ����
            //services.AddAuthentication(options =>//����Ĭ��ע��,�Զ������Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //�����ø�Cookie����
            //    ;
            //services.CustomAddAuthenticationCore();//�滻��IOC,������Ժ���չ


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("DateOfBirthPolicy", policyBuilder => policyBuilder.Requirements.Add(new DateOfBirthRequirement()));
            //    options.AddPolicy("CountryChinesePolicy", policyBuilder => policyBuilder.Requirements.Add(new CountryRequirement("Chinese")));
            //    options.AddPolicy("CountryChinaPolicy", policyBuilder => policyBuilder.Requirements.Add(new CountryRequirement("China")));

            //    options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));
            //});
            //services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, QQMailHandler>();

            //services.AddSingleton<IAuthorizationHandler, DateOfBirthRequirementHandler>();
            //services.AddSingleton<IAuthorizationHandler, CountryRequirementHandler>();
            #endregion

            #region ��handler  
            //ͬSchemeֻ��һ��
            //services.AddAuthentication(options =>
            //    {
            //        options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //        options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//������
            //        options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;

            //        options.RequireAuthenticatedSignIn = false;
            //    })
            //;
            //services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme + "1";//������
            //     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme + "1";
            //     options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme + "1";
            //     options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme + "1";
            //     options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme + "1";

            //     options.RequireAuthenticatedSignIn = false;
            // })
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme + "1", options =>
            //{
            //    options.LoginPath = "/Auth/Login1";
            //    options.Cookie.Name = "www1";
            //})
            //;
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme + "2", options =>
            //{
            //    options.LoginPath = "/Auth/Login2";
            //    options.Cookie.Name = "www2";
            //})
            //.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = "Zhaoxi.NET6.DemoProject",
            //        ValidAudience = "Zhaoxi.NET6.DemoProject",
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Eleven.Zhaoxi.NET6.DemoProject"))
            //    };
            //});
            #endregion

            #region Filter��ʽ
            //services.AddAuthentication()
            //.AddCookie();
            #endregion

            #region ������Ȩ����---����Cookies��Ȩ��Ȩ--�����AddAuthorization--����+��ɫ��Ȩ
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.LoginPath = "/Authorization/LoginPath";
            //    options.AccessDeniedPath = "/Authorization/AccessDeniedPath";
            //});

            ////services.AddAuthorization();//��AddController�����Ѿ�����,���Բ�д
            ////ָ��һ��Policy
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("Admin")//Claim��Role��Admin
            //        );

            //    options.AddPolicy("UserPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("User")//Claim��Role��User
            //        );
            //});
            #endregion

            #region ���Դ��
            ////2��Scheme����
            //services.AddAuthentication(options =>//ʹ���Զ����UrlToken����Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //�����ø�Cookie����
            //;
            //services.CustomAddAuthenticationCore();//�滻��IOC,������Ժ���չ


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder =>
            //        {
            //            policyBuilder.RequireRole("Admin");////Claim��Role��Admin
            //            policyBuilder.RequireUserName("Eleven");
            //        });

            //    options.AddPolicy("UserPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("User")//Claim��Role��User
            //        );

            //    options.AddPolicy("MutiPolicy", policyBuilder =>
            //    {
            //        policyBuilder
            //        .RequireRole("Admin")//Claim��Role��Admin----���ձ���Ϊ����һ��Requirement(Ҫ��-����)
            //        .RequireUserName("Eleven")//Claim��Name��Eleven
            //        .RequireClaim(ClaimTypes.Email)//������ĳ��Cliam
            //        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)//���Դ��������--ָ�����û���Ϣ��Դ
            //        .Combine(new AuthorizationPolicyBuilder().AddRequirements(new SingleEmailRequirement("@qq.com")).Build())//QQ����Ҫ��  ͬ��
            //        .RequireAssertion(context =>
            //            context.User.HasClaim(c => c.Type == ClaimTypes.Role)
            //            && context.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "Admin")//������Ȩ�����������Զ���������
            //        ;//����

            //        policyBuilder.Requirements.Add(new DoubleEmailRequirement());//2ѡ1 ����--��Ҫע��IAuthorizationHandler
            //        policyBuilder.Requirements.Add(new SingleEmailRequirement("@qq.com"));//��һ������Ҫע��IAuthorizationHandler
            //    });

            //    options.AddPolicy("DateOfBirthPolicy", policyBuilder => policyBuilder.Requirements.Add(new DateOfBirthRequirement()));
            //    options.AddPolicy("CountryChinesePolicy", policyBuilder => policyBuilder.Requirements.Add(new CountryRequirement("Chinese")));
            //    options.AddPolicy("CountryChinaPolicy", policyBuilder => policyBuilder.Requirements.Add(new CountryRequirement("China")));
            //    options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));

            //});

            //services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, DateOfBirthRequirementHandler>();
            //services.AddSingleton<IAuthorizationHandler, CountryRequirementHandler>();

            #endregion

            #region �Զ����Ȩhandler �ѵ�Cookie--��handler---������Ȩ����
            //services.AddAuthentication(options =>//����Ĭ��ע��,�Զ������Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //�����ø�Cookie����
            //    ;
            //services.CustomAddAuthenticationCore();//�滻��IOC,������Ժ���չ


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("DateOfBirthPolicy", policyBuilder => policyBuilder.Requirements.Add(new DateOfBirthRequirement()));
            //    options.AddPolicy("CountryChinesePolicy", policyBuilder => policyBuilder.Requirements.Add(new CountryRequirement("Chinese")));
            //    options.AddPolicy("CountryChinaPolicy", policyBuilder => policyBuilder.Requirements.Add(new CountryRequirement("China")));
            //    options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));
            //});

            //services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, DateOfBirthRequirementHandler>();
            //services.AddSingleton<IAuthorizationHandler, CountryRequirementHandler>();
            #endregion

            #region Cookie��Ȩ+Policy+Requirements+IAuthorizationHandler��չ
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//������,signin signout Authenticate���ǻ���Scheme
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.LoginPath = "/Authorization/LoginPath";
            //    options.AccessDeniedPath = "/Authorization/AccessDeniedPath";
            //});

            ////����һ�����õ�AuthorizationPolicy
            //var qqEmailPolicy = new AuthorizationPolicyBuilder().AddRequirements(new QQEmailRequirement()).Build();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("Admin")//Claim��Role��Admin
            //        .RequireUserName("Eleven")//Claim��Name��Eleven
            //        .RequireClaim(ClaimTypes.Email)//������ĳ��Cliam
            //        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)//���Դ��������
            //        .AddRequirements(new QQEmailRequirement())//QQ����Ҫ��
            //        .Combine(qqEmailPolicy)//QQ����Ҫ��  ͬ��
            //        .RequireAssertion(context =>
            //            context.User.HasClaim(c => c.Type == ClaimTypes.Role)
            //            && context.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "Admin")//������Ȩ�����������Զ���������
            //        );//����

            //    options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));
            //});
            //services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion

            #region ����Cookie��Ȩ
            //services.AddScoped<ITicketStore, MemoryCacheTicketStore>();
            //services.AddMemoryCache();
            //////services.AddDistributedRedisCache(options =>
            //////{
            //////    options.Configuration = "127.0.0.1:6379";
            //////    options.InstanceName = "RedisDistributedSession";
            //////});
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//������
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = "Cookie/Login";
            //})
            //.AddCookie(options =>
            //{
            //    //��Ϣ���ڷ����--��keyд��cookie--����session
            //    options.SessionStore = services.BuildServiceProvider().GetService<ITicketStore>();
            //    options.Events = new CookieAuthenticationEvents()
            //    {
            //        OnSignedIn = new Func<CookieSignedInContext, Task>(
            //            async context =>
            //            {
            //                Console.WriteLine($"{context.Request.Path} is OnSignedIn");
            //                await Task.CompletedTask;
            //            }),
            //        OnSigningIn = async context =>
            //         {
            //             Console.WriteLine($"{context.Request.Path} is OnSigningIn");
            //             await Task.CompletedTask;
            //         },
            //        OnSigningOut = async context =>
            //        {
            //            Console.WriteLine($"{context.Request.Path} is OnSigningOut");
            //            await Task.CompletedTask;
            //        }
            //    };//��չ�¼�
            //});

            ////new AuthenticationBuilder().AddCookie()
            #endregion


            #region AddJWT---������Token������

            #region jwtУ��  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            Configuration.Bind("JWTTokenOptions", tokenOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
                    ValidateIssuer = true,//�Ƿ���֤Issuer
                    ValidateAudience = true,//�Ƿ���֤Audience
                    ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                    ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey

                    ValidAudience = tokenOptions.Audience,//Audience,��Ҫ��ǰ��ǩ��jwt������һ��
                    ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//�õ�SecurityKey

                    //AudienceValidator = (m, n, z) =>
                    //{
                    //    //��ͬ��ȥ��չ����Audience��У�����---��Ȩ
                    //    return m != null && m.FirstOrDefault().Equals(this.Configuration["audience"]);
                    //},
                    //LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    //{
                    //    return notBefore <= DateTime.Now
                    //    && expires >= DateTime.Now;
                    //    //&& validationParameters
                    //}//�Զ���У�����
                };
                #region Events
                //���ṩ��ί����չ��Ҳ����ֱ��new�¶���override����
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("JWTAuthenticationFailed", "expired");//���߿ͻ����ǹ�����
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnChallenge");
                        context.Response.Headers.Add("JWTChallenge", "expired");//���߿ͻ����ǹ�����
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnForbidden");
                        context.Response.Headers.Add("JWTForbidden", "expired");//���߿ͻ����ǹ�����
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnMessageReceived");
                        context.Response.Headers.Add("JWTMessageReceived", "expired");//���߿ͻ����ǹ�����
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnTokenValidated");
                        context.Response.Headers.Add("JWTTokenValidated", "expired");//���߿ͻ����ǹ�����
                        return Task.CompletedTask;
                    }
                };
                #endregion
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ComplicatedPolicy",
                    policyBuilder => policyBuilder
                    .RequireRole("Admin")//Claim��Role��Admin
                    .RequireUserName("Eleven")//Claim��Name��Eleven
                    //.RequireClaim(ClaimTypes.Email)//������ĳ��Cliam--����
                    .RequireClaim("EMail")//������ĳ��Cliam-����
                    .RequireClaim("Account")
                    //.RequireAssertion(context =>
                    //    context.User.HasClaim(c => c.Type == "Role")
                    //    && context.User.Claims.First(c => c.Type.Equals("Role")).Value.Equals("Assistant", StringComparison.OrdinalIgnoreCase))
                    );//����

                options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder
                .Requirements.Add(new DoubleEmailRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion

            #region jwtУ��  RS

            //JWTTokenOptions tokenOptions = new JWTTokenOptions();
            //Configuration.Bind("JWTTokenOptions", tokenOptions);//�����SecurityKey��ʵû��������,��������Ĺ�Կ

            //string path = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            //string key = File.ReadAllText(path);
            //Console.WriteLine($"KeyPath:{path}");
            //var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            //var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,//�Ƿ���֤Issuer
            //        ValidateAudience = true,//�Ƿ���֤Audience
            //        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
            //        ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
            //        ValidAudience = tokenOptions.Audience,//Audience,��Ҫ��ǰ��ǩ��jwt������һ��
            //        ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
            //        IssuerSigningKey = new RsaSecurityKey(keyParams),
            //        //IssuerSigningKeyValidator = (m, n, z) =>
            //        // {
            //        //     Console.WriteLine("This is IssuerValidator");
            //        //     return true;
            //        // },
            //        //IssuerValidator = (m, n, z) =>
            //        // {
            //        //     Console.WriteLine("This is IssuerValidator");
            //        //     return "http://localhost:5726";
            //        // },
            //        //AudienceValidator = (m, n, z) =>
            //        //{
            //        //    Console.WriteLine("This is AudienceValidator");
            //        //    return true;
            //        //    //return m != null && m.FirstOrDefault().Equals(this.Configuration["Audience"]);
            //        //},//�Զ���У����򣬿����µ�¼��֮ǰ����Ч
            //    };

            //    #region Events
            //    ////���ṩ��ί����չ��Ҳ����ֱ��new�¶���override����
            //    //options.Events = new JwtBearerEvents()
            //    //{
            //    //    OnAuthenticationFailed = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
            //    //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //    //        {
            //    //            context.Response.Headers.Add("JWTAuthenticationFailed", "expired");//���߿ͻ����ǹ�����
            //    //        }
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnChallenge = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnChallenge");
            //    //        context.Response.Headers.Add("JWTChallenge", "expired");//���߿ͻ����ǹ�����
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnForbidden = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnForbidden");
            //    //        context.Response.Headers.Add("JWTForbidden", "expired");//���߿ͻ����ǹ�����
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnMessageReceived = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnMessageReceived");
            //    //        context.Response.Headers.Add("JWTMessageReceived", "expired");//���߿ͻ����ǹ�����
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnTokenValidated = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnTokenValidated");
            //    //        context.Response.Headers.Add("JWTTokenValidated", "expired");//���߿ͻ����ǹ�����
            //    //        return Task.CompletedTask;
            //    //    }
            //    //};
            //    #endregion
            //});
            #endregion

            #endregion
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

            #region ��Ȩ��Ȩ
            #region Ĭ�Ͼ���
            //app.UseAuthorization();//Ĭ�Ͼ���--Add��AddController���Ѿ������
            #endregion

            #region ��׼���
            app.UseAuthentication();//Ĭ�Ͽ��û��--�ͱ�������Add
            app.UseAuthorization();
            #endregion

            #region �滻���
            //app.CustomUseAuthentication();
            //app.CustomUseAuthorization();
            #endregion

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
