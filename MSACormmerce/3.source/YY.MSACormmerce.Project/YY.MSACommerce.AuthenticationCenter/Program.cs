using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YY.AgileFramework.Common.Models;
using YY.AgileFramework.Core.ConsulExtend;
using YY.AgileFramework.Core.ConsulExtend.DispatcherExtend;
using YY.AgileFramework.Core.ConsulExtend.ServerExtend;
using YY.AgileFramework.Core.ConsulExtend.ServerExtend.Register;
using YY.AgileFramework.WebCore.JWTExtend;
using YY.MSACommerce.DTOModel.DTO;
using YY.MSACormmerce.AuthenticationCenter.Utility;

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
.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.AddFilter("System", LogLevel.Warning);
    loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
    loggingBuilder.AddLog4Net();
});
#endregion

#region ServiceRegister
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region 接口调用
builder.Services.AddTransient<HttpHelperService>();
builder.Services.AddTransient<AbstractConsulDispatcher, AverageDispatcher>();
#endregion

//需要JWT的封装
#region JWT
#region HS256 对称可逆加密
//builder.Services.AddScoped<IJWTService, JWTHSService>();
//builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

//多了个Extension封装，屏蔽下注册和初始化细节
builder.Services.AddJWTBuilder(JWTAlgorithmType.HS256, () =>
{
    builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
});
#endregion

#region RS256 非对称可逆加密，需要获取一次公钥
////string keyDir = Directory.GetCurrentDirectory();
////if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
////{
////    keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
////}

////builder.Services.AddScoped<IJWTService, JWTRSService>();
////builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

//builder.Services.AddJWTBuilder(JWTAlgorithmType.HS256, () =>
//{
//    builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
//});
#endregion
#endregion

#region Consul Server IOC注册
builder.Services.Configure<ConsulRegisterOptions>(builder.Configuration.GetSection("ConsulRegisterOption"));
builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("ConsulClientOption"));
builder.Services.AddConsulRegister();
#endregion


#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region MinimalAPI
//接受账号密码---去用户微服务验证---生成token
#region 获取Token

app.MapPost("/api/auth/accredit", (
    ILogger<Program> logger,
    [FromServices] IJWTService jwtService,
    HttpHelperService httpHelperService,
    AbstractConsulDispatcher abstractConsulDispatcher,  //IOC注入
    [FromForm] string username, //获取参数
    [FromForm] string password
    ) =>
{
    //string requestUrl = $"{builder.Configuration["VerifyUserUrl"]}?username={username}&password={password}";
    //string realUrl = requestUrl;//this._IConsulDispatcher.GetAddress(requestUrl);
    //基于Consul去获取地址信息---只有IP:Port--然后调用
    string url = $"{builder.Configuration["VerifyUserUrl"]}";
    url = abstractConsulDispatcher.MapAddress(url);
    string realUrl = $"{url}?username={username}&password={password}";
    Console.WriteLine($"{url}--{realUrl}");
    AjaxResult<DTOJWTUser> ajaxResult = httpHelperService.VerifyUser(realUrl);
    if (ajaxResult.Result)
    {
        var dtoUser = ajaxResult.TValue;

        string token = jwtService.GetToken(new JWTUserModel()
        {
            id = dtoUser.id,
            username = dtoUser.username
        });
        ajaxResult.Value = token;
    }
    Console.WriteLine($"Accredit Result : {JsonConvert.SerializeObject(ajaxResult)}");

    //return new JsonResult(ajaxResult);//这样会被多包一层
    return ajaxResult;
});

#endregion

#region 获取refreshToken
app.MapPost("/api/auth/accreditWithRefresh",
    (ILogger<Program> logger,
    IJWTService jwtService,
    HttpHelperService httpHelperService,
    AbstractConsulDispatcher abstractConsulDispatcher,  //IOC注入
    IHttpContextAccessor httpContextAccessor,
    string? username, //获取参数
    string? password
    )
    =>
    {
        Console.WriteLine($"This is LoginWithRefresh {username}--{password}");
        var httpContext = httpContextAccessor.HttpContext;
        username = "yp";
        password = "123456";

        //string requestUrl = $"{builder.Configuration["VerifyUserUrl"]}?username={username}&password={password}";
        //string realUrl = requestUrl;//this._IConsulDispatcher.GetAddress(requestUrl);

        string url = $"{builder.Configuration["VerifyUserUrl"]}";
        url = abstractConsulDispatcher.MapAddress(url);
        string realUrl = $"{url}?username={username}&password={password}";

        AjaxResult<DTOJWTUser> ajaxResult = httpHelperService.VerifyUser(realUrl);
        if (ajaxResult.Result)
        {
            var dtoUser = ajaxResult.TValue;

            var tokenPair = jwtService.GetTokenWithRefresh(new JWTUserModel()
            {
                id = dtoUser.id,
                username = dtoUser.username
            });
            if (tokenPair != null && !string.IsNullOrEmpty(tokenPair.Item1))
            {
                ajaxResult.Value = tokenPair.Item1;
                ajaxResult.OtherValue = tokenPair.Item2;
            }
            else
            {
                ajaxResult.Result = false;
                ajaxResult.Message = "颁发token失败";
            }
        }
        Console.WriteLine($"Accredit Result : {JsonConvert.SerializeObject(ajaxResult)}");
        return ajaxResult;
    });

app.MapPost("/api/auth/refresh",
     (ILogger<Program> logger,
     IJWTService jwtService,
     IHttpContextAccessor httpContextAccessor,
     string? refreshToken
     )
     =>
     {
         var httpContext = httpContextAccessor.HttpContext;
         Console.WriteLine($"This is refresh {refreshToken}");
         if (!refreshToken.ValidateRefreshToken())
         {
             return new AjaxResult()
             {
                 Result = false,
                 Message = "refreshToken过期了"
             };
         }
         else
         {
             var token = jwtService.GetTokenByRefresh(refreshToken);
             return new AjaxResult()
             {
                 Result = true,
                 Value = token
             };
         }
     });
#endregion

#region Consul注册
app.UseHealthCheckMiddleware("/Health");//心跳请求响应
app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
#endregion

#endregion
app.Run();
