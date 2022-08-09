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

            #region ConfigureBuilder---��������
            //builder.Configuration.AddJsonFile
            builder.Host
            //.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            //{
            //    LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);
            //    configurationBuilder
            //        .AddApollo(configurationBuilder.Build().GetSection("apollo"))
            //        .AddDefault()
            //        .AddNamespace("ZhaoxiMSAPrivateJson", ConfigFileFormat.Json)//�Զ����private NameSpace
            //        .AddNamespace(ConfigConsts.NamespaceApplication);//Apollo��Ĭ��NameSpace������
            //})
            ////����Զ��������ļ���Ĭ��Ϊappsettings.json
            //.ConfigureAppConfiguration((hostingContext, config) =>
            //{
            //    config.Sources.Clear();
            //    config.AddJsonFile($"myapp.json", optional: false, reloadOnChange: true);
            //})
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                // Ĭ�ϻ��ȡ�����ļ���Ϊlog4net.Config���ļ�
                //����ʹ���Զ����ļ��������贫�����������loggingBuilder.AddLog4Net("MyTestLog4.Config");�����ļ������������MyTestLog4.Config
                loggingBuilder.AddLog4Net();
            });           
            #endregion

            #region ServiceRegister
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //֧��Token
            builder.Services.AddSwaggerGen(options =>
            {
                #region Swagger����֧��Token�������� 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "������token,��ʽΪ Bearer jwtToken(ע���м�����пո�)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });//��Ӱ�ȫ����
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {   //��Ӱ�ȫҪ��
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


            #region ����ע��
            builder.Services.AddTransient<OrangeContext, OrangeContext>();
            builder.Services.AddTransient<CacheClientDB, CacheClientDB>();
            builder.Services.AddTransient<IUserService, UserService>();
            #endregion

            #region �����ļ�ע��
            builder.Services.Configure<MySqlConnOptions>(builder.Configuration.GetSection("MysqlConn"));
            builder.Services.Configure<RedisConnOptions>(builder.Configuration.GetSection("RedisConn"));
            #endregion

            #region jwtУ��  HS
            JWTTokenOptions tokenOptions = new JWTTokenOptions();
            builder.Configuration.Bind("JWTTokenOptions", tokenOptions);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //JWT��һЩĬ�ϵ����ԣ����Ǹ���Ȩʱ�Ϳ���ɸѡ��
                    ValidateIssuer = true,//�Ƿ���֤Issuer
                    ValidateAudience = true,//�Ƿ���֤Audience
                    ValidateLifetime = false,//�Ƿ���֤ʧЧʱ��
                    ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                    ValidAudience = tokenOptions.Audience,//
                    ValidIssuer = tokenOptions.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
                };
            });
            #endregion

            #region ����ȡ���������ṩ����
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

            #region Consul����ע��
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
            app.UseHealthCheckMiddleware("/Health");//����������Ӧ
            app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
            #endregion

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            #region ����ȡ���������ṩ����
            //app.UseCors("default");
            #endregion

            app.MapControllers();

            app.Run();
        }
    }
}