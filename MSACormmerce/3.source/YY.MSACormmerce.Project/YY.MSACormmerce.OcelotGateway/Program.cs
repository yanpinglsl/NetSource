using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Polly;
using Ocelot.Middleware;
using YY.AgileFramework.WebCore.MiddlewareExtend;

var builder = WebApplication.CreateBuilder(args);

//�����һ��Json����Դ
builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

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
.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.AddFilter("System", LogLevel.Warning);
    //loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
    loggingBuilder.AddLog4Net();
});
#endregion

#region ServiceRegister
// Add services to the container.

builder.Services.AddControllers();
#region Swagger
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
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
#endregion

#region Ocelot
builder.Services.AddOcelot()//Ocelot��δ���
  .AddConsul()//֧��Consul
  .AddCacheManager(x =>
  {
      x.WithDictionaryHandle();//Ĭ���ֵ�洢
  })
  .AddPolly()
  ;
#endregion

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ocelot V1");
            c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "AuthenticationCenter  WebAPI V1");
            c.SwaggerEndpoint("/user/swagger/v1/swagger.json", "UserMicroservice  WebAPI V1");
        });
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

#region OptionsԤ������
app.UsePreOptionsRequest();
#endregion


#region ����ocelot
app.UseOcelot();//ֱ���滻�˹ܵ�ģ��
#endregion

app.Run();
