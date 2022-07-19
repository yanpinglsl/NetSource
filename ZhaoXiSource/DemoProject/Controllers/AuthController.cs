using ExtendLib.AuthExtend;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<AuthController> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public AuthController(IConfiguration configuration
            , ILoggerFactory loggerFactory
            , ILogger<AuthController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
            this._loggerFactory = loggerFactory;
        }

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/Auth
        /// </summary>
        /// <returns></returns>
        [Authorize] //表示需要授权，没有任何规则，只要求有用户信息
        public IActionResult Index()
        {
            this._loggerFactory.CreateLogger<AuthController>().LogWarning("This is AuthController-Index 1");

            var endpoint = base.HttpContext.GetEndpoint();

            Console.WriteLine(base.HttpContext.Items["__AuthorizationMiddlewareWithEndpointInvoked"]);

            return View();
        }

        #region  基于Cookie基本鉴权-授权基本流程
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IActionResult> Login(string name, string password)
        {
            //base.HttpContext.RequestServices.
            //IAuthenticationService

            if ("Eleven".Equals(name, StringComparison.CurrentCultureIgnoreCase)
                && password.Equals("123456"))
            {
                var claimIdentity = new ClaimsIdentity("Custom");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "57265177@qq.com"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.Country, "Chinese"));
                claimIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, "1986"));
                await base.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                });//登陆默认Scheme，写入Cookie
                return new JsonResult(new
                {
                    Result = true,
                    Message = "登录成功"
                });
            }
            else
            {
                await Task.CompletedTask;
                return new JsonResult(new
                {
                    Result = false,
                    Message = "登录失败"
                });
            }
        }
        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await base.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new
            {
                Result = true,
                Message = "退出成功"
            });
        }

        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IActionResult> Authentication()
        {
            //Console.WriteLine($"base.HttpContext.User?.Claims?.First()?.Value={base.HttpContext.User?.Claims?.First()?.Value}");

            var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal != null)
            {
                base.HttpContext.User = result.Principal;
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证成功，包含用户{base.HttpContext.User.Identity.Name}"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"认证失败，用户未登录"
                });
            }
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Authorization()
        {
            var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"认证失败，用户未登录"
                });
            }
            else
            {
                base.HttpContext.User = result.Principal;
            }

            //授权
            var user = base.HttpContext.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                if (!user.Identity.Name.Equals("Eleven", StringComparison.OrdinalIgnoreCase))
                {
                    await base.HttpContext.ForbidAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return new JsonResult(new
                    {
                        Result = false,
                        Message = $"授权失败，用户{base.HttpContext.User.Identity.Name}没有权限"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        Result = true,
                        Message = $"授权成功，用户{base.HttpContext.User.Identity.Name}具备权限"
                    });
                }
            }
            else
            {
                await base.HttpContext.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"授权失败，没有登录"
                });
            }
        }

        /// <summary>
        /// 需要授权的页面
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Info()
        {
            var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证失败，用户未登录"
                });
            }
            else
            {
                base.HttpContext.User = result.Principal;
            }

            //授权
            var user = base.HttpContext.User;
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                if (!user.Identity.Name.Equals("Eleven", StringComparison.OrdinalIgnoreCase))
                {
                    await base.HttpContext.ForbidAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return new JsonResult(new
                    {
                        Result = false,
                        Message = $"授权失败，用户{base.HttpContext.User.Identity.Name}没有权限"
                    });
                }
                else
                {
                    //有权限
                    return new JsonResult(new
                    {
                        Result = true,
                        Message = $"授权成功，正常访问页面！",
                        Html = "Hello Root!"
                    });
                }
            }
            else
            {
                await base.HttpContext.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return new JsonResult(new
                {
                    Result = false,
                    Message = $"授权失败，没有登录"
                });
            }
        }
        #endregion

        #region 自定义UrlToken
        ///// <summary>
        ///// http://localhost:5726/Auth/UrlToken
        ///// http://localhost:5726/Auth/UrlToken?UrlToken=eleven-123456
        ///// </summary>
        ///// <returns></returns>
        //public async Task<IActionResult> UrlToken()
        //{
        //    var result = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
        //    if (result?.Principal == null)
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = false,
        //            Message = $"认证失败，用户未登录"
        //        });
        //    }
        //    else
        //    {
        //        base.HttpContext.User = result.Principal;
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"认证失败，用户未登录"
        //        });
        //    }

        //    //授权
        //    var user = base.HttpContext.User;
        //    if (user?.Identity?.IsAuthenticated ?? false)
        //    {
        //        if (!user.Identity.Name.Equals("Eleven", StringComparison.OrdinalIgnoreCase))
        //        {
        //            await base.HttpContext.ForbidAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
        //            return new JsonResult(new
        //            {
        //                Result = false,
        //                Message = $"授权失败，用户{base.HttpContext.User.Identity.Name}没有权限"
        //            });
        //        }
        //        else
        //        {
        //            //有权限
        //            return new JsonResult(new
        //            {
        //                Result = true,
        //                Message = $"授权成功，正常访问页面！",
        //                Html = "Hello Root!"
        //            });
        //        }
        //    }
        //    else
        //    {
        //        await base.HttpContext.ChallengeAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
        //        return new JsonResult(new
        //        {
        //            Result = false,
        //            Message = $"授权失败，没有登录"
        //        });
        //    }
        //}

        ///// <summary>
        ///// http://localhost:5726/Auth/AuthorizeData
        ///// http://localhost:5726/Auth/AuthorizeData?UrlToken=eleven-123456
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //public async Task<IActionResult> AuthorizeData()
        //{
        //    await base.HttpContext.SignOutAsync("UrlTokenScheme");
        //    return new JsonResult(new
        //    {
        //        Result = true,
        //        Message = "退出成功"
        //    });
        //}
        #endregion

        #region 多Scheme UrlToken+Cookie
        ///// <summary>
        ///// http://localhost:5726/Auth/UrlCookieByDefault
        ///// http://localhost:5726/Auth/UrlCookieByDefault?UrlToken=eleven-123456
        ///// </summary>
        ///// <returns></returns>
        //[Authorize()]//为空，则是默认(UrlToken)--甚至可以不要
        //public async Task<IActionResult> UrlCookieByDefault()
        //{
        //    Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");

        //    var result = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
        //    if (result?.Principal == null)
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"UrlCookieByDefault 认证失败，用户未登录"
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"UrlCookieByDefault 认证成功，有用户信息"
        //        });
        //    }
        //}

        ///// <summary>
        ///// http://localhost:5726/Auth/UrlCookieByUrlToken
        ///// http://localhost:5726/Auth/UrlCookieByUrlToken?UrlToken=eleven-123456
        ///// </summary>
        ///// <returns></returns>
        //[Authorize(AuthenticationSchemes = UrlTokenAuthenticationDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> UrlCookieByUrlToken()
        //{
        //    Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");

        //    var result = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
        //    if (result?.Principal == null)
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"UrlCookieByUrlToken 认证失败，用户未登录"
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"UrlCookieByUrlToken 认证成功，有用户信息"
        //        });
        //    }
        //}

        ///// <summary>
        ///// http://localhost:5726/Auth/UrlCookieByCookie
        ///// http://localhost:5726/Auth/UrlCookieByCookie?UrlToken=eleven-123456
        ///// </summary>
        ///// <returns></returns>
        ////[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        ////[Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{UrlTokenAuthenticationDefaults.AuthenticationScheme}")]//多个Scheme 其实授权时信息可以共享

        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + UrlTokenAuthenticationDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> UrlCookieByCookie()
        //{
        //    Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");

        //    var urlToken = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
        //    Console.WriteLine($"urlToken?.Principal == null ={urlToken?.Principal == null}");


        //    var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    if (result?.Principal == null)
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"认证失败，用户未登录"
        //        });
        //    }
        //    else
        //    {
        //        return new JsonResult(new
        //        {
        //            Result = true,
        //            Message = $"认证成功，有用户信息"
        //        });
        //    }
        //}

        #region 多Scheme带混合授权
        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// 
        /// http://localhost:5726/Auth/UrlCookieByMuti
        /// http://localhost:5726/Auth/UrlCookieByMuti?UrlToken=eleven-123456
        /// </summary>
        /// <returns></returns>

        // http://localhost:5726/Auth/UrlCookieByMuti 一切正常，都能满足,Policy和Roles是And并且关系
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "Admin")]
        // http://localhost:5726/Auth/UrlCookieByMuti 一切正常，都能满足，多个Role是并列关系
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "Admin,User")]
        // http://localhost:5726/Auth/UrlCookieByMuti  roles不对,访问失败，跳到授权失败(带上UrlToken也不行)
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "User")]
        //http://localhost:5726/Auth/UrlCookieByMuti?UrlToken=eleven-123456  roles不对,访问失败，跳到授权失败(带上UrlToken参数也不行)
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "User")]
        ////http://localhost:5726/Auth/UrlCookieByMuti?UrlToken=eleven-123456  多个Scheme解析，数据共享，Policy是Cookie的，Roles是UrlToken的--能成功
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + UrlTokenAuthenticationDefaults.AuthenticationScheme, Policy = "CountryChinesePolicy", Roles = "User")]
        //http://localhost:5726/Auth/UrlCookieByMuti?UrlToken=eleven-123456 换Scheme，一切正确，
        //[Authorize(AuthenticationSchemes = UrlTokenAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "User")]
        //http://localhost:5726/Auth/UrlCookieByMuti 支持双重校验，2个Policy都必须满足
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "Admin")]
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "CountryChinesePolicy", Roles = "User")]
        ////http://localhost:5726/Auth/UrlCookieByMuti?UrlToken=eleven-123456 支持双重校验,不同的Scheme，cookie和token缺一不可
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "Admin")]
        //[Authorize(AuthenticationSchemes = UrlTokenAuthenticationDefaults.AuthenticationScheme, Policy = "DateOfBirthPolicy", Roles = "User")]
        ////http://localhost:5726/Auth/UrlCookieByMuti?UrlToken=eleven-123456 双重校验基础上,不同的Scheme，其实邮箱不一样，但DoubleEmail都能满足
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "DoubleEmail", Roles = "Admin")]
        [Authorize(AuthenticationSchemes = UrlTokenAuthenticationDefaults.AuthenticationScheme, Policy = "DoubleEmail", Roles = "User")]

        public async Task<IActionResult> UrlCookieByMuti()
        {
            Console.WriteLine($"主动鉴权之前：base.HttpContext.User?.Claims?.First()?.Value == null?{base.HttpContext.User?.Claims?.First()?.Value == null}");

            var urlToken = await base.HttpContext.AuthenticateAsync(UrlTokenAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"urlToken?.Principal == null ={urlToken?.Principal == null}");

            var result = await base.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证失败，用户未登录"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    Result = true,
                    Message = $"认证成功，有用户信息"
                });
            }
        }



        ///// <summary>
        ///// JWT
        ///// </summary>
        ///// <returns></returns>
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public IActionResult Api()
        //{
        //    return new JsonResult(new
        //    {
        //        Name = HttpContext.User.Identity.Name,
        //        Message = "Succeed"
        //    });
        //}
        ///// <summary>
        ///// 颁发jwt token
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public string GetApiToken(string name)
        //{
        //    List<Claim> claims = new List<Claim> {
        //        new Claim(ClaimTypes.Name,name)
        //    };
        //    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Eleven.Zhaoxi.NET6.DemoProject"));//对称加密
        //    JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
        //        issuer: "Zhaoxi.NET6.DemoProject"
        //        , audience: "Zhaoxi.NET6.DemoProject"
        //        , claims: claims
        //        , notBefore: DateTime.UtcNow
        //        , expires: DateTime.UtcNow.AddMinutes(10)
        //        , signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        //    );
        //    return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        //}
        #endregion
        #endregion

    }
}
