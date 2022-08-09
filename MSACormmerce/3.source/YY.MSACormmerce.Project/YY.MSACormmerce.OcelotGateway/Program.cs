using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Polly;
using Ocelot.Middleware;
using YY.AgileFramework.WebCore.MiddlewareExtend;

var builder = WebApplication.CreateBuilder(args);

//多添加一个Json数据源
builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

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
#endregion

#region Ocelot
builder.Services.AddOcelot()//Ocelot如何处理
  .AddConsul()//支持Consul
  .AddCacheManager(x =>
  {
      x.WithDictionaryHandle();//默认字典存储
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

#region Options预请求处理
app.UsePreOptionsRequest();
#endregion


#region 常规ocelot
app.UseOcelot();//直接替换了管道模型
#endregion

app.Run();
