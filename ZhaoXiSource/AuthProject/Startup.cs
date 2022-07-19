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

            //////services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展

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

            //////services.CustomAddAuthenticationCore();//替换下IOC,方便调试和扩展
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
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            Configuration.Bind("JWTTokenOptions", tokenOptions);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = true,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey

                    ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致
                    ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),//拿到SecurityKey

                    //AudienceValidator = (m, n, z) =>
                    //{
                    //    //等同于去扩展了下Audience的校验规则---鉴权
                    //    return m != null && m.FirstOrDefault().Equals(this.Configuration["audience"]);
                    //},
                    //LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    //{
                    //    return notBefore <= DateTime.Now
                    //    && expires >= DateTime.Now;
                    //    //&& validationParameters
                    //}//自定义校验规则
                };
                #region Events
                //即提供了委托扩展，也可以直接new新对象，override方法
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnAuthenticationFailed");
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("JWTAuthenticationFailed", "expired");//告诉客户端是过期了
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnChallenge");
                        context.Response.Headers.Add("JWTChallenge", "expired");//告诉客户端是过期了
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnForbidden");
                        context.Response.Headers.Add("JWTForbidden", "expired");//告诉客户端是过期了
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnMessageReceived");
                        context.Response.Headers.Add("JWTMessageReceived", "expired");//告诉客户端是过期了
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"This JWT Authentication OnTokenValidated");
                        context.Response.Headers.Add("JWTTokenValidated", "expired");//告诉客户端是过期了
                        return Task.CompletedTask;
                    }
                };
                #endregion
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ComplicatedPolicy",
                    policyBuilder => policyBuilder
                    .RequireRole("Admin")//Claim的Role是Admin
                    .RequireUserName("Eleven")//Claim的Name是Eleven
                    //.RequireClaim(ClaimTypes.Email)//必须有某个Cliam--不行
                    .RequireClaim("EMail")//必须有某个Cliam-可以
                    .RequireClaim("Account")
                    //.RequireAssertion(context =>
                    //    context.User.HasClaim(c => c.Type == "Role")
                    //    && context.User.Claims.First(c => c.Type.Equals("Role")).Value.Equals("Assistant", StringComparison.OrdinalIgnoreCase))
                    );//内置

                options.AddPolicy("DoubleEmail", policyBuilder => policyBuilder
                .Requirements.Add(new DoubleEmailRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, ZhaoxiMailHandler>();
            services.AddSingleton<IAuthorizationHandler, QQMailHandler>();
            #endregion

            #region jwt校验  RS

            //JWTTokenOptions tokenOptions = new JWTTokenOptions();
            //Configuration.Bind("JWTTokenOptions", tokenOptions);//这里的SecurityKey其实没有意义了,换成下面的公钥

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
            //        ValidateIssuer = true,//是否验证Issuer
            //        ValidateAudience = true,//是否验证Audience
            //        ValidateLifetime = true,//是否验证失效时间
            //        ValidateIssuerSigningKey = true,//是否验证SecurityKey
            //        ValidAudience = tokenOptions.Audience,//Audience,需要跟前面签发jwt的设置一致
            //        ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
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
            //        //},//自定义校验规则，可以新登录后将之前的无效
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
    }
}
