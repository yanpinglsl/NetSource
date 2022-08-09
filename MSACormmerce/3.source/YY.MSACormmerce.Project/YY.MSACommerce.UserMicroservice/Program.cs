using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using YY.AgileFramework.Common.IOCOptions;
using YY.AgileFramework.Core.ConsulExtend;
using YY.AgileFramework.Core.ConsulExtend.ServerExtend;
using YY.AgileFramework.Core.ConsulExtend.ServerExtend.Register;
using YY.AgileFramework.WebCore.JWTExtend;
using YY.MSACommerce.Core;
using YY.MSACommerce.Interface;
using YY.MSACommerce.Model;
using YY.MSACommerce.Service;

namespace YY.MSACommerce.UserMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region ConfigureBuilder---基础配置
            //builder.Configuration.AddJsonFile
            builder.Host
            //.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            //{
            //    LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
            //    configurationBuilder
            //        .AddApollo(configurationBuilder.Build().GetSection("apollo"))
            //        .AddDefault()
            //        .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//自定义的private NameSpace
            //        .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo中默认NameSpace的名称
            //})
            ////添加自定义配置文件，默认为appsettings.json
            //.ConfigureAppConfiguration((hostingContext, config) =>
            //{
            //    config.Sources.Clear();
            //    config.AddJsonFile($"myapp.json", optional: false, reloadOnChange: true);
            //})
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                // 默认会读取配置文件名为log4net.Config的文件
                //如需使用自定义文件名，则需传入参数，例如loggingBuilder.AddLog4Net("MyTestLog4.Config");配置文件名则必须命名MyTestLog4.Config
                loggingBuilder.AddLog4Net();
            });           
            #endregion

            #region ServiceRegister
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //支持Token
            builder.Services.AddSwaggerGen(options =>
            {
                #region Swagger配置支持Token参数传递 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token,格式为 Bearer jwtToken(注意中间必须有空格)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });//添加安全定义
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {   //添加安全要求
                    new OpenApiSecurityScheme
                    {
                        Reference =new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id ="Bearer"
                        }
                    },
                    new string[]{ }
                }
                });
                #endregion
            });


            #region 服务注入
            builder.Services.AddTransient<OrangeContext, OrangeContext>();
            builder.Services.AddTransient<CacheClientDB, CacheClientDB>();
            builder.Services.AddTransient<IUserService, UserService>();
            #endregion

            #region 配置文件注入
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisConnOptions>(builder.Configuration.GetSection("RedisConn"));
            #endregion

            #region jwt校验  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                    ValidateIssuer = true,//是否验证Issuer
                    ValidateAudience = true,//是否验证Audience
                    ValidateLifetime = false,//是否验证失效时间
                    ValidateIssuerSigningKey = true,//是否验证SecurityKey
                    ValidAudience = tokenOptions.Audience,//
                    ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                };
            });
            #endregion

            #region 跨域取消，网关提供配置
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("default", policy =>
            //    {
            //        //policy.WithOrigins(new string[] { "http://localhost:8070", "http://api.yitao.com" })
            //        policy.AllowAnyOrigin()
            //            .AllowAnyHeader()
            //            .AllowAnyMethod();
            //        //.AllowCredentials();
            //    });
            //});
            #endregion

            #region Consul服务注册
            builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
            builder.Services.AddConsulRegister();
            #endregion
            #endregion


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            #region Consul
            app.UseHealthCheckMiddleware("/Health");//心跳请求响应
            app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            #region 跨域取消，网关提供配置
            //app.UseCors("default");
            #endregion

            app.MapControllers();

            app.Run();
        }
    }
}