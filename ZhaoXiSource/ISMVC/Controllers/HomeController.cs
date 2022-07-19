using ISMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ISMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string accessToken = HttpContext.GetTokenAsync("access_token").Result;
            string idToken = HttpContext.GetTokenAsync("id_token").Result;
            var claimsList = from c in User.Claims select new { c.Type, c.Value };
            return View();
        }
        public IActionResult Logout()
        {
            //清楚本地Cookie以及Iddentity Server缓存
            return SignOut("Cookies", "oidc");
        }
    }
}
