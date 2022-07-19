﻿using AuthProject.Utility;
using Common.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthProject.Controllers
{
    public class TokenController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<TokenController> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public TokenController(IConfiguration configuration
            , ILoggerFactory loggerFactory
            , ILogger<TokenController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
            this._loggerFactory = loggerFactory;
        }

        /// <summary>
        /// http://localhost:5726/token/index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// http://localhost:5726/Token/Login?Name=Eleven&Password=123456
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Login(string name, string password)
        {
            #region 之前写的本地登陆，这里访问数据库
            //if ("Eleven".Equals(name, StringComparison.CurrentCultureIgnoreCase)
            //    && password.Equals("123456"))
            //{
            //    var claimIdentity = new ClaimsIdentity("Custom");
            //    claimIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
            //    //claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "xuyang@ZhaoxiEdu.Net"));
            //    claimIdentity.AddClaim(new Claim(ClaimTypes.Email, "57265177@qq.com"));
            //    claimIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

            //    await base.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), new AuthenticationProperties
            //    {
            //        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
            //    });//登录为默认的scheme  cookies
            //    return new JsonResult(new
            //    {
            //        Result = true,
            //        Message = "登录成功"
            //    });
            //}
            //else
            //{
            //    await Task.CompletedTask;
            //    return new JsonResult(new
            //    {
            //        Result = false,
            //        Message = "登录失败"
            //    });
            //}
            #endregion

            #region 换成调用服务登陆，获取Token
            Console.WriteLine($"This is Login name={name} password={password}");

            var result = JWTTokenHelper.IssueToken(name, password);

            Console.WriteLine($"This is Login result={Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
            await Task.CompletedTask;
            return new JsonResult(result);
            #endregion
        }

        //[Authorize]//表示需要授权
        //[Authorize(Roles = "Admin,User")]//表示需要授权
        //[Authorize(Roles = "Administrator")]//表示需要授权
        [Authorize(Policy = "ComplicatedPolicy")]
        [HttpGet]
        public async Task<IActionResult> InfoGet()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in base.HttpContext.User.Identities.First().Claims)
            {
                Console.WriteLine($"InfoGet {item.Type}:{item.Value}");
                sb.Append($"{item.Type}:{item.Value}");
            }
            await Task.CompletedTask;
            return new JsonResult(
                new AjaxResult<string>()
                {
                    Result = true,
                    TValue = sb.ToString()
                });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InfoPost()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in base.HttpContext.User.Identities.First().Claims)
            {
                Console.WriteLine($"InfoPost {item.Type}:{item.Value}");
                sb.Append($"{item.Type}:{item.Value}");
            }
            await Task.CompletedTask;
            return new JsonResult(
                new AjaxResult<string>()
                {
                    Result = true,
                    TValue = sb.ToString()
                });
        }

        /// <summary>
        /// http://localhost:5726/Token/Login?Name=Eleven&Password=123456
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRefresh(string name, string password)
        {
            #region 换成调用服务登陆，获取Token
            Console.WriteLine($"This is LoginWithRefresh name={name} password={password}");

            var result = JWTTokenHelper.IssueTokenWithRefresh(name, password);

            Console.WriteLine($"This is LoginWithRefresh result={Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
            await Task.CompletedTask;
            return new JsonResult(result);
            #endregion
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginByRefresh([FromForm] string refreshToken)
        {
            #region 换成调用服务登陆，获取Token
            Console.WriteLine($"This is LoginByRefresh refreshToken={refreshToken}");

            var result = JWTTokenHelper.IssueTokenByRefresh(refreshToken);

            Console.WriteLine($"This is LoginByRefresh result={Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
            await Task.CompletedTask;
            return new JsonResult(result);
            #endregion
        }
    }
}
