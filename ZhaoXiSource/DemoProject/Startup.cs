using Autofac;
using DemoProject.Utility;
using ExtendLib.ActionExtend.Argument;
using ExtendLib.AuthExtend;
using ExtendLib.AuthExtend.Requirement;
using ExtendLib.ControllerExtend;
using ExtendLib.ResultExtend;
using ExtendLib.ViewResultExtend;
using JWT.AuthenticationCenter.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoProject
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
            services.AddControllersWithViews(options => {

                #region ArgumentBinder
                //需要插入到第一条,内置默认是匹配到合适的Provider就不会在向下继续绑定;如果添加到末尾,即不会调用到我们实现的
                options.ModelBinderProviders.Insert(0, new StringTrimModelBinderProvider(options.InputFormatters));
                #endregion

                #region ResultFormatter
                //插入到Json之前
                //options.OutputFormatters.Insert(3, new NameValueOutputFormatter());
                //options.OutputFormatters.Insert(3, new UserOutputFormatter());
                #endregion

                #region ModelConvention
                //options.Conventions.Add(new CustomControllerModelConvention());
                //options.Conventions.Add(new CustomActionModelConvention());
                //options.Conventions.Add(new CustomParameterModelConvention());
                #endregion

                #region 缓存
                options.CacheProfiles.Add("default1", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 30,  // 30s
                });

                options.CacheProfiles.Add("default2", new Microsoft.AspNetCore.Mvc.CacheProfile
                {
                    Duration = 60,  // 60s
                                         //Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.Any,
                                         //NoStore = true,
                                         //VaryByHeader = "User-Agent",
                                         //VaryByQueryKeys = new string[] { "aaa" }
                });
                #endregion
            })
                .AddControllersAsServices();//服务化;
            #region 本地化服务,可将网站本地化为不同的语言文化
            services.AddLocalization(o =>
            {
                o.ResourcesPath = "Resources";//指定了文件夹Resources作为存放翻译文件的目录
            });
            #endregion
            #region Options
            //1、
            //services.Configure<EmailOption>(op => op.Title = "services.Configure<EmailOption>--DefaultName");//默认--名称empty
            //services.Configure<EmailOption>("FromMemory", op => op.Title = "services.Configure<EmailOption>---FromMemory");//指定名称,程序里面配置
            //services.Configure<EmailOption>("FromConfiguration", Configuration.GetSection("Email"));//从配置文件读取

            //services.Configure<EmailOption>("FromConfigurationNew", Configuration.GetSection("EmailNew"));//从配置文件读取

            //services.AddOptions<EmailOption>("AddOption").Configure(op => op.Title = "AddOption Title--DefaultName");//等价于Configure
            //services.Configure<EmailOption>(null, op => op.From = "services.Configure<EmailOption>--Name null--Same With ConfigureAll");
            ////services.ConfigureAll<EmailOption>(op => op.From = "ConfigureAll");

            //services.PostConfigure<EmailOption>(null, op => op.Body = "services.PostConfigure<EmailOption>--Name null--Same With PostConfigureAll");

            ////services.PostConfigureAll<EmailOption>(op => op.Body = "PostConfigurationAll");

            ////2、解除对IOptions接口的依赖后,可以在不引用Microsoft.Extensions.Options程序集的情况下，
            ////注入了一个“原始”(EmailOption)的配置对象
            //services.Configure<EmailOption>(Configuration.GetSection("EmailNew"));//从配置文件读取
            //services.AddSingleton(resolver =>
            //    resolver.GetRequiredService<IOptions<EmailOption>>().Value);
            #endregion

            #region IStartupFilter拓展

            #region IStartupFilter拓展1
            //services.AddTransient<IStartupFilter, CustomStartupFilter>();//ConfigureServices先执行的
            #endregion

            #region IStartupFilter拓展2(强类型配置对象添加验证)
            //services.AddTransient<IStartupFilter, SettingValidationStartupFilter>();
            //services.Configure<SlackApiSettings>(Configuration.GetSection("SlackApi"));

            ////解除对IOptions接口的依赖后,可以在不引用Microsoft.Extensions.Options程序集的情况下，注入了一个“原始”的配置对象
            //services.AddSingleton(resolver =>
            //    resolver.GetRequiredService<IOptions<SlackApiSettings>>().Value);

            //services.AddSingleton<IValidatable>(resolver =>
            //    resolver.GetRequiredService<IOptions<SlackApiSettings>>().Value);
            #endregion

            #endregion

            #region Middleware扩展
            //services.AddSingleton<SecondMiddleWare>();
            //services.Replace(ServiceDescriptor.Singleton<IMiddlewareFactory, SecondMiddleWareFactory>());//替换默认容器
            #endregion

            #region Middleware标准封装
            //services.AddBrowserFilter();//玩法1

            //services.AddBrowserFilter(option =>
            //{
            //    option.EnableIE = false;
            //    option.EnableFirefox = true;
            //    option.EnableChorme = true;
            //});//玩法2
            //services.AddBrowserFilter(option =>
            //{
            //    option.EnableIE = false;
            //    option.EnableFirefox = true;
            //    option.EnableChorme = true;
            //});//玩法3
            #endregion

            #region 静态文件之文件夹浏览
            services.AddDirectoryBrowser();
            #endregion

            #region 分布式缓存--Session
            //services.AddSession();
            ////Nuget Microsoft.Extensions.Caching.Redis
            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = "127.0.0.1:6379";
            //    options.InstanceName = "RedisDistributedCache";
            //});
            #endregion

            #region 鉴权授权  同时生效只有一个

            #region 基本鉴权流程---配置
            ////services.AddAuthentication();//没有任何Scheme不行，程序要求有DefaultScheme
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//需要鉴权，且必须指定默认方案
            //         .AddCookie();//使用Cookie的方式
            ////services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展
            #endregion

            #region 自定义鉴权Handler
            ////使用Url参数传递用户信息
            //services.AddAuthentication(options =>
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");

            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //});//覆盖默认注册,自定义解析Handler

            ////services.AddAuthentication(UrlTokenAuthenticationDefaults.AuthenticationScheme)
            ////.AddCookie();

            ////services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展

            #endregion

            #region 自定义鉴权handler 堆叠Cookie--多handler
            //services.AddAuthentication(options =>//覆盖默认注册,自定义解析Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme); //再配置个Cookie解析

            ////services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展
            #endregion

            #region 自定义鉴权handler 堆叠Cookie--多handler---加上授权策略
            //services.AddAuthentication(options =>//覆盖默认注册,自定义解析Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //再配置个Cookie解析
            //    ;
            ////services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展

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

            #region 多handler  
            //同Scheme只能一个
            //services.AddAuthentication(options =>
            //    {
            //        options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //        options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//不能少
            //        options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //        options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;

            //        options.RequireAuthenticatedSignIn = false;
            //    })
            //;
            //services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme + "1";//不能少
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

            #region Filter方式
            //services.AddAuthentication()
            //.AddCookie();
            #endregion

            #region 基础授权流程---基于Cookies鉴权授权--最基础AddAuthorization--策略+角色授权
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.LoginPath = "/Authorization/LoginPath";
            //    options.AccessDeniedPath = "/Authorization/AccessDeniedPath";
            //});

            ////services.AddAuthorization();//在AddController里面已经有了,可以不写
            ////指定一个Policy
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("Admin")//Claim的Role是Admin
            //        );

            //    options.AddPolicy("UserPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("User")//Claim的Role是User
            //        );
            //});
            #endregion

            #region 解读源码
            ////2个Scheme叠加
            //services.AddAuthentication(options =>//使用自定义的UrlToken解析Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //再配置个Cookie解析
            //;
            //services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展


            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder =>
            //        {
            //            policyBuilder.RequireRole("Admin");////Claim的Role是Admin
            //            policyBuilder.RequireUserName("Eleven");
            //        });

            //    options.AddPolicy("UserPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("User")//Claim的Role是User
            //        );

            //    options.AddPolicy("MutiPolicy", policyBuilder =>
            //    {
            //        policyBuilder
            //        .RequireRole("Admin")//Claim的Role是Admin----最终表现为就是一个Requirement(要求-条件)
            //        .RequireUserName("Eleven")//Claim的Name是Eleven
            //        .RequireClaim(ClaimTypes.Email)//必须有某个Cliam
            //        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)//可以从这里添加--指定的用户信息来源
            //        .Combine(new AuthorizationPolicyBuilder().AddRequirements(new SingleEmailRequirement("@qq.com")).Build())//QQ邮箱要求  同上
            //        .RequireAssertion(context =>
            //            context.User.HasClaim(c => c.Type == ClaimTypes.Role)
            //            && context.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "Admin")//根据授权处理上下文自定义规则检验
            //        ;//内置

            //        policyBuilder.Requirements.Add(new DoubleEmailRequirement());//2选1 都行--需要注册IAuthorizationHandler
            //        policyBuilder.Requirements.Add(new SingleEmailRequirement("@qq.com"));//单一，不需要注册IAuthorizationHandler
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

            #region 自定义鉴权handler 堆叠Cookie--多handler---加上授权策略
            //services.AddAuthentication(options =>//覆盖默认注册,自定义解析Handler
            //{
            //    options.AddScheme<UrlTokenAuthenticationHandler>(UrlTokenAuthenticationDefaults.AuthenticationScheme, "UrlTokenScheme-Demo");
            //    options.DefaultAuthenticateScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultChallengeScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultForbidScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignOutScheme = UrlTokenAuthenticationDefaults.AuthenticationScheme;
            //})
            //   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //再配置个Cookie解析
            //    ;
            //services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展


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

            #region Cookie鉴权+Policy+Requirements+IAuthorizationHandler扩展
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//不能少,signin signout Authenticate都是基于Scheme
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //{
            //    options.LoginPath = "/Authorization/LoginPath";
            //    options.AccessDeniedPath = "/Authorization/AccessDeniedPath";
            //});

            ////定义一个共用的AuthorizationPolicy
            //var qqEmailPolicy = new AuthorizationPolicyBuilder().AddRequirements(new QQEmailRequirement()).Build();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("Admin")//Claim的Role是Admin
            //        .RequireUserName("Eleven")//Claim的Name是Eleven
            //        .RequireClaim(ClaimTypes.Email)//必须有某个Cliam
            //        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)//可以从这里解析
            //        .AddRequirements(new QQEmailRequirement())//QQ邮箱要求
            //        .Combine(qqEmailPolicy)//QQ邮箱要求  同上
            //        .RequireAssertion(context =>
            //            context.User.HasClaim(c => c.Type == ClaimTypes.Role)
            //            && context.User.Claims.First(c => c.Type.Equals(ClaimTypes.Role)).Value == "Admin")//根据授权处理上下文自定义规则检验
            //        );//内置

            //    options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder.Requirements.Add(new DoubleEmailRequirement()));
            //});
            //services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion

            #region 基于Cookie鉴权
            //services.AddScoped<ITicketStore, MemoryCacheTicketStore>();
            //services.AddMemoryCache();
            //////services.AddDistributedRedisCache(options =>
            //////{
            //////    options.Configuration = "127.0.0.1:6379";
            //////    options.InstanceName = "RedisDistributedSession";
            //////});
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;//不能少
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = "Cookie/Login";
            //})
            //.AddCookie(options =>
            //{
            //    //信息存在服务端--把key写入cookie--类似session
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
            //    };//扩展事件
            //});

            ////new AuthenticationBuilder().AddCookie()
            #endregion

            #region AddJWT---仅访问Token控制器

            #region jwt校验  HS
            //JWTTokenOptions tokenOptions = new JWTTokenOptions();
            //Configuration.Bind("JWTTokenOptions", tokenOptions);

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        //JWT有一些默认的属性，就是给鉴权时就可以筛选了
            //        ValidateIssuer = true,//是否验证Issuer
            //        ValidateAudience = true,//是否验证Audience
            //        ValidateLifetime = true,//是否验证失效时间
            //        ValidateIssuerSigningKey = true,//是否验证SecurityKey

            //        ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致
            //        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到SecurityKey

            //        //AudienceValidator = (m, n, z) =>
            //        //{
            //        //    //等同于去扩展了下Audience的校验规则---鉴权
            //        //    return m != null && m.FirstOrDefault().Equals(this.Configuration["audience"]);
            //        //},
            //        //LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
            //        //{
            //        //    return notBefore <= DateTime.Now
            //        //    && expires >= DateTime.Now;
            //        //    //&& validationParameters
            //        //}//自定义校验规则
            //    };
            //    #region Events
            //    ////即提供了委托扩展，也可以直接new新对象，override方法
            //    //options.Events = new JwtBearerEvents()
            //    //{
            //    //    OnAuthenticationFailed = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
            //    //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //    //        {
            //    //            context.Response.Headers.Add("JWTAuthenticationFailed", "expired");//告诉客户端是过期了
            //    //        }
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnChallenge = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnChallenge");
            //    //        context.Response.Headers.Add("JWTChallenge", "expired");//告诉客户端是过期了
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnForbidden = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnForbidden");
            //    //        context.Response.Headers.Add("JWTForbidden", "expired");//告诉客户端是过期了
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnMessageReceived = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnMessageReceived");
            //    //        context.Response.Headers.Add("JWTMessageReceived", "expired");//告诉客户端是过期了
            //    //        return Task.CompletedTask;
            //    //    },
            //    //    OnTokenValidated = context =>
            //    //    {
            //    //        Console.WriteLine($"This JWT Authentication OnTokenValidated");
            //    //        context.Response.Headers.Add("JWTTokenValidated", "expired");//告诉客户端是过期了
            //    //        return Task.CompletedTask;
            //    //    }
            //    //};
            //    #endregion
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ComplicatedPolicy",
            //        policyBuilder => policyBuilder
            //        .RequireRole("Admin")//Claim的Role是Admin
            //        .RequireUserName("Eleven")//Claim的Name是Eleven
            //        //.RequireClaim(ClaimTypes.Email)//必须有某个Cliam--不行
            //        .RequireClaim("EMail")//必须有某个Cliam-可以
            //        .RequireClaim("Account")
            //        //.RequireAssertion(context =>
            //        //    context.User.HasClaim(c => c.Type == "Role")
            //        //    && context.User.Claims.First(c => c.Type.Equals("Role")).Value.Equals("Assistant", StringComparison.OrdinalIgnoreCase))
            //        );//内置

            //    options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder
            //    .Requirements.Add(new DoubleEmailRequirement()));
            //});
            //services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            //services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion

            #region jwt校验  RS

            ////JWTTokenOptions tokenOptions = new JWTTokenOptions();
            ////Configuration.Bind("JWTTokenOptions", tokenOptions);//这里的SecurityKey其实没有意义了,换成下面的公钥

            ////string path = Path.Combine(Directory.GetCurrentDirectory(), "key.public.json");
            ////string key = File.ReadAllText(path);
            ////Console.WriteLine($"KeyPath:{path}");
            ////var keyParams = JsonConvert.DeserializeObject<RSAParameters>(key);
            ////var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            ////services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            ////.AddJwtBearer(options =>
            ////{
            ////    options.TokenValidationParameters = new TokenValidationParameters
            ////    {
            ////        ValidateIssuer = true,//是否验证Issuer
            ////        ValidateAudience = true,//是否验证Audience
            ////        ValidateLifetime = true,//是否验证失效时间
            ////        ValidateIssuerSigningKey = true,//是否验证SecurityKey
            ////        ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致
            ////        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
            ////        IssuerSigningKey = new RsaSecurityKey(keyParams),
            ////        //IssuerSigningKeyValidator = (m, n, z) =>
            ////        // {
            ////        //     Console.WriteLine("This is IssuerValidator");
            ////        //     return true;
            ////        // },
            ////        //IssuerValidator = (m, n, z) =>
            ////        // {
            ////        //     Console.WriteLine("This is IssuerValidator");
            ////        //     return "http://localhost:5726";
            ////        // },
            ////        //AudienceValidator = (m, n, z) =>
            ////        //{
            ////        //    Console.WriteLine("This is AudienceValidator");
            ////        //    return true;
            ////        //    //return m != null && m.FirstOrDefault().Equals(this.Configuration["Audience"]);
            ////        //},//自定义校验规则，可以新登录后将之前的无效
            ////    };

            ////    #region Events
            ////    ////即提供了委托扩展，也可以直接new新对象，override方法
            ////    //options.Events = new JwtBearerEvents()
            ////    //{
            ////    //    OnAuthenticationFailed = context =>
            ////    //    {
            ////    //        Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
            ////    //        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            ////    //        {
            ////    //            context.Response.Headers.Add("JWTAuthenticationFailed", "expired");//告诉客户端是过期了
            ////    //        }
            ////    //        return Task.CompletedTask;
            ////    //    },
            ////    //    OnChallenge = context =>
            ////    //    {
            ////    //        Console.WriteLine($"This JWT Authentication OnChallenge");
            ////    //        context.Response.Headers.Add("JWTChallenge", "expired");//告诉客户端是过期了
            ////    //        return Task.CompletedTask;
            ////    //    },
            ////    //    OnForbidden = context =>
            ////    //    {
            ////    //        Console.WriteLine($"This JWT Authentication OnForbidden");
            ////    //        context.Response.Headers.Add("JWTForbidden", "expired");//告诉客户端是过期了
            ////    //        return Task.CompletedTask;
            ////    //    },
            ////    //    OnMessageReceived = context =>
            ////    //    {
            ////    //        Console.WriteLine($"This JWT Authentication OnMessageReceived");
            ////    //        context.Response.Headers.Add("JWTMessageReceived", "expired");//告诉客户端是过期了
            ////    //        return Task.CompletedTask;
            ////    //    },
            ////    //    OnTokenValidated = context =>
            ////    //    {
            ////    //        Console.WriteLine($"This JWT Authentication OnTokenValidated");
            ////    //        context.Response.Headers.Add("JWTTokenValidated", "expired");//告诉客户端是过期了
            ////    //        return Task.CompletedTask;
            ////    //    }
            ////    //};
            ////    #endregion
            ////});
            #endregion

            #endregion

            #endregion


            #region 控制器控制
            #region Activator替换
            ////IControllerActivator的默认实现是DefaultControllerActivator
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, CustomControllerActivator>());//将容器注册好的IControllerActivator给换成自己的
            #endregion

            #region ControllerFactory替换
            //IControllerFactory的默认实现是DefaultControllerFactory
            //services.Replace(ServiceDescriptor.Transient<IControllerFactory, CustomControllerFactory>());//将容器注册好的IControllerFactory给换成自己的
            #endregion
            #endregion


            #region Result扩展
            //services.Replace(ServiceDescriptor.Transient<IActionResultExecutor<JsonResult>, NewtonsoftJsonActionResultExecutor>());

            //services.AddTransient<IActionResultExecutor<NameValueResult>, NameValueResultExecutor>();
            #endregion

            #region ViewResult扩展路径
            //services.Configure<RazorViewEngineOptions>(options =>
            //{
            //    //options.ViewLocationExpanders.Add(new CustomViewLocationExpander());
            //    options.ViewLocationExpanders.Insert(0, new CustomViewLocationExpander());
            //});
            #endregion

            #region HttpContext获取
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//如果需要获取HttpContext
            #endregion


            #region 中间件缓存
            //services.AddResponseCaching(option =>
            //{
            //    //option.UseCaseSensitivePaths = true;//大小写
            //    //option.MaximumBodySize = 100;//最大体积
            //    //option.SizeLimit = 65;//单个最大
            //});
            #endregion

            #region 本地缓存
            services.AddMemoryCache(options =>
            {
                options.Clock = new LocalClock();
                //options.SizeLimit = 1000;
            });

            #endregion

            #region 分布式缓存
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "127.0.0.1:6379";
                options.InstanceName = "RedisDistributedCache";
            });
            #endregion
        }
        
        private class LocalClock : ISystemClock
        {
            public DateTimeOffset UtcNow => DateTime.Now;
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                #region 使用框架自带异常中间件
                //app.UseExceptionHandler(builder => builder.Use(ExceptionHandlerDemo));
                #endregion
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                #region  扩展指定错误处理动作
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");//只要不是200 都能进来
                //app.UseExceptionHandler(errorApp =>
                //{
                //    errorApp.Run(async context =>
                //    {
                //        context.Response.StatusCode = 200;
                //        context.Response.ContentType = "text/html";

                //        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
                //        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

                //        var exceptionHandlerPathFeature =
                //            context.Features.Get<IExceptionHandlerPathFeature>();

                //        Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                //        Console.WriteLine($"{exceptionHandlerPathFeature?.Error.Message}");
                //        Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

                //        // Use exceptionHandlerPathFeature to process the exception (for example, 
                //        // logging), but do NOT expose sensitive error information directly to 
                //        // the client.

                //        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                //        {
                //            await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
                //        }

                //        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
                //        await context.Response.WriteAsync("</body></html>\r\n");
                //        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
                //    });
                //});
                //app.UseHsts();
                #endregion
            }

            #region 自定义异常处理中间件
            //app.UseExceptionMiddleware();
            #endregion


            #region 添加Log4Net或者自定义的
            //loggerFactory.AddLog4Net();
            //loggerFactory.AddColorConsoleLogger();
            //loggerFactory.AddColorConsoleLogger(new CustomColorConsoleLoggerConfiguration
            //{
            //    LogLevel = LogLevel.Warning,
            //    Color = ConsoleColor.Blue
            //});
            //loggerFactory.AddColorConsoleLogger(c =>
            //{
            //    c.LogLevel = LogLevel.Information;
            //    c.Color = ConsoleColor.Blue;
            //});
            #endregion

            //注意：MVC中如果没有配置UseStatic则静态文件请求也会进入管道，因此会导致中间件、IstartFilter等重复执行。WebAPI则不会，因为是纯后端
            #region 最原生Use中间件
            //代码有几层？一共有3层
            //app.Use(
            //   new Func<RequestDelegate, RequestDelegate>(
            //   next =>
            //   {
            //       Console.WriteLine("This is middleware 0.1");
            //       return new RequestDelegate(
            //            async context =>
            //       {
            //           await context.Response.WriteAsync("This is Hello World 0.1 start");
            //           await next.Invoke(context);
            //           await context.Response.WriteAsync("This is Hello World 0.1   end");

            //           await Task.Run(() => Console.WriteLine("12345678797989"));
            //       });
            //   }));

            //app.Use(
            //    new Func<RequestDelegate, RequestDelegate>(
            //    next =>
            //{
            //    Console.WriteLine("This is middleware 1");
            //    return new RequestDelegate(
            //        async context =>
            //        {
            //            await context.Response.WriteAsync("This is Hello World 1 start");
            //            //context.Response.OnStarting(state =>
            //            //{
            //            //    var httpContext = (HttpContext)state;
            //            //    httpContext.Response.Headers.Add("middleware", "12345");
            //            //    return Task.CompletedTask;
            //            //}, context);
            //            await next.Invoke(context);
            //            await context.Response.WriteAsync("This is Hello World 1   end");

            //            await Task.Run(() => Console.WriteLine("12345678797989"));
            //        });
            //}));

            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 2");
            //    return new RequestDelegate(
            //        async context =>
            //        {
            //            await context.Response.WriteAsync("This is Hello World 2 start");
            //            await next.Invoke(context);
            //            await context.Response.WriteAsync("This is Hello World 2   end");
            //        });
            //});
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 3");
            //    return new RequestDelegate(
            //        async context =>
            //        {
            //            await context.Response.WriteAsync("This is Hello World 3 start");
            //            //await next.Invoke(context);//最后这个没有执行Next---如果也执行该句则报错
            //            await context.Response.WriteAsync("This is The Chooen One!");
            //            await context.Response.WriteAsync("This is Hello World 3   end");
            //        });
            //});
            #endregion

            #region Use、UseWhen、Map、MapWhen
            ////传入的委托中含有一个委托参数，我们一般用next来接收这个委托参数，通过调用next.Invode()方法来调用下一个中间件，这样Use之后的中间件才能够被执行
            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("use1 start");
            //    await next.Invoke();
            //    Console.WriteLine("use1 end");
            //});
            //////Run相当于一个终结点，Run之后的中间件不会被执行，因为它不像Use一样可以调用next.Invoke();
            ////app.Run(context =>
            ////{
            ////    Console.WriteLine("run");
            ////   return context.Response.WriteAsync("Run,Hello World!");
            ////});
            ////Map中间件最明显的特征是只有访问特定的路径才会执行
            //app.Map("/map", app =>
            //{
            //    app.Run(context =>
            //    {
            //        Console.WriteLine("map");
            //        return context.Response.WriteAsync("Map,Hello World!");
            //    });
            //});
            ////MapWhen，当条件成立时，中间件才会被执行，并且MapWhen创建了一个新的管道，当满足条件时，新的管道会代替主管道，这意味着主管道的中间件不会被执行
            ////app.MapWhen(context =>
            ////{
            ////    return context.Request.Query.ContainsKey("Name");
            ////    //拒绝非chorme浏览器的请求  
            ////    //多语言
            ////    //把ajax统一处理
            ////}, MapTest);
            ////app.MapWhen(context =>
            ////{
            ////    return context.Request.Query.ContainsKey("Name");
            ////    //拒绝非chorme浏览器的请求  
            ////    //多语言
            ////    //把ajax统一处理
            ////}, app =>
            ////{
            ////    app.Use(async (context, next) =>
            ////    {
            ////        Console.WriteLine("mapwhen1 start ");
            ////        await next.Invoke();
            ////        Console.WriteLine("mapwhen1 end");
            ////        await context.Response.WriteAsync("Url is " + context.Request.PathBase.ToString());
            ////    });
            ////});
            ////UseWhen和MapWhen类似，也是当条件成立时，中间件才会被执行，区别是UseWhen不会代替主管道
            //app.UseWhen(context => true, app =>
            //{
            //    app.Use(async (context, next) =>
            //    {
            //        Console.WriteLine("usewhen1 start");
            //        await next.Invoke();
            //        Console.WriteLine("usewhen1 end");
            //    });
            //});
            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("use2 start ");
            //    await next.Invoke();
            //    Console.WriteLine("use2 end ");
            //});
            #endregion

            #region UseMiddleware式
            ////UseMiddlerware 类--反射找
            //app.UseMiddleware<FirstMiddleWare>();
            //app.UseMiddleware<SecondMiddleWare>();
            //app.UseMiddleware<ThreeMiddleWare>("Eleven Zhaoxi.NET6.DemoProject");
            #endregion

            #region 标准Middleware
            ////玩法1---Use传递---Add就无操作---IOptions<BrowserFilterOptions> options就是Use指定传递的 
            ////玩法2---Use不传递---靠Add实现---IOptions<BrowserFilterOptions> options就是IOC生成的
            ////玩法3---都传递---Use和Add都传递--Add为准1  Use为准2   叠加3---结果是2，以Use为准，原因是对象只会是UseMiddleware传递的值，就不会再找IOC了---但是合理吗？----可以升级注入IConfigureOptions<BrowserFilterOptions>，然后叠加生效

            //app.UseBrowserFilter(new BrowserFilterOptions()
            //{
            //    EnableIE = false,
            //    EnableFirefox = false
            //});//玩法1

            //app.UseBrowserFilter();//玩法2

            //app.UseBrowserFilter(new BrowserFilterOptions()
            //{
            //    EnableIE = true,
            //    EnableFirefox = false
            //});//玩法3
            //app.Use(next =>
            //{
            //    Console.WriteLine("This is middleware 3");
            //    return new RequestDelegate(
            //        async context =>
            //        {
            //            await context.Response.WriteAsync("This is Hello World 3 start");
            //            //await next.Invoke(context);//最后这个没有执行Next---带着呢
            //            await context.Response.WriteAsync("This is The Chooen One!");
            //            await context.Response.WriteAsync("This is Hello World 3   end");
            //        });
            //});
            #endregion

            //app.UseHttpsRedirection();

            #region 静态文件
            //app.UseRefuseStealing();

            app.UseStaticFiles();//使用默认文件夹wwwroot  

            //app.UseRefuseStealing();//写在这里就没用了，因为前面已经处理了呀--很多中间件的顺序都是有原因的

            ////增加文件访问权限
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
            //    RequestPath = new PathString("/MyImages")
            //});//dotnet  和dotnet run

            ////增加文件夹访问权限
            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            //    RequestPath = "/CsutomImages"
            //});
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    OnPrepareResponse = context =>
            //    {
            //        context.Context.Response.Headers[HeaderNames.CacheControl] = "no-store";//"no-cache";//
            //    }
            //});
            #endregion

            #region 中间件缓存
            app.UseResponseCaching();
            #endregion

            #region Session
            //app.UseSession();//不是默认的，默认都没有Session
            #endregion

            #region 本地化中间件
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("zh-CN"),
                new CultureInfo("ja-JP"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,//指定当前应用支持的所有语言文化(指定的是数字和日期格式)
                SupportedUICultures = supportedCultures //指定当前应用支持的所有语言文化(指定的翻译文件)
            });
            #endregion
            app.UseRouting();

            #region 鉴权授权
            #region 默认就有
            //app.UseAuthorization();//默认就有--Add在AddController就已经添加了
            #endregion

            #region 标准组合
            app.UseAuthentication();//默认框架没有--就必须配套Add
            app.UseAuthorization();
            #endregion

            #region 替换组合
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

        private void MapTest(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Url is " + context.Request.PathBase.ToString());
            });
        }
        private async Task ExceptionHandlerDemo(HttpContext httpContext, Func<Task> next)
        {
            //该信息由ExceptionHandlerMiddleware中间件提供，里面包含了ExceptionHandlerMiddleware中间件捕获到的异常信息。
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;

            if (ex != null)
            {
                httpContext.Response.ContentType = "application/problem+json";

                var title = "An error occured: " + ex.Message;
                var details = ex.ToString();

                var problem = new 
                {
                    Status = 500,
                    Title = title,
                    Detail = details
                };

                var stream = httpContext.Response.Body;
                await JsonSerializer.SerializeAsync(stream, problem);
            }
        }
        #region Autofac容器
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            //containerBuilder.RegisterType<TestServiceB>().As<ITestServiceB>().SingleInstance();
            containerBuilder.RegisterModule<CustomAutofacModule>();
        }
        #endregion

    }
}
