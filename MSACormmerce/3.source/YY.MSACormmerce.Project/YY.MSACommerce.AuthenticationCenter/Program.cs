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
    loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
    loggingBuilder.AddLog4Net();
});
#endregion

#region ServiceRegister
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region �ӿڵ���
builder.Services.AddTransient<HttpHelperService>();
builder.Services.AddTransient<AbstractConsulDispatcher, AverageDispatcher>();
#endregion

//��ҪJWT�ķ�װ
#region JWT
#region HS256 �Գƿ������
//builder.Services.AddScoped<IJWTService, JWTHSService>();
//builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

//���˸�Extension��װ��������ע��ͳ�ʼ��ϸ��
builder.Services.AddJWTBuilder(JWTAlgorithmType.HS256, () =>
{
    builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
});
#endregion

#region RS256 �ǶԳƿ�����ܣ���Ҫ��ȡһ�ι�Կ
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

#region Consul Server IOCע��
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
//�����˺�����---ȥ�û�΢������֤---����token
#region ��ȡToken

app.MapPost("/api/auth/accredit", (
    ILogger<Program> logger,
    [FromServices] IJWTService jwtService,
    HttpHelperService httpHelperService,
    AbstractConsulDispatcher abstractConsulDispatcher,  //IOCע��
    [FromForm] string username, //��ȡ����
    [FromForm] string password
    ) =>
{
    //string requestUrl = $"{builder.Configuration["VerifyUserUrl"]}?username={username}&password={password}";
    //string realUrl = requestUrl;//this._IConsulDispatcher.GetAddress(requestUrl);
    //����Consulȥ��ȡ��ַ��Ϣ---ֻ��IP:Port--Ȼ�����
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

    //return new JsonResult(ajaxResult);//�����ᱻ���һ��
    return ajaxResult;
});

#endregion

#region ��ȡrefreshToken
app.MapPost("/api/auth/accreditWithRefresh",
    (ILogger<Program> logger,
    IJWTService jwtService,
    HttpHelperService httpHelperService,
    AbstractConsulDispatcher abstractConsulDispatcher,  //IOCע��
    IHttpContextAccessor httpContextAccessor,
    string? username, //��ȡ����
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
                ajaxResult.Message = "�䷢tokenʧ��";
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
                 Message = "refreshToken������"
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

#region Consulע��
app.UseHealthCheckMiddleware("/Health");//����������Ӧ
app.Services.GetService<IConsulRegister>()!.UseConsulRegist().Wait();
#endregion

#endregion
app.Run();
