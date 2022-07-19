using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Practice.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class FirstController : Controller
    {
        public IConfiguration _Configuration { get; }

        private ICompanyService _ICompanyService = null;


        /// <summary>
        /// 依赖注入：
        /// 1.构造函数注入---.NET5内置的容器仅支持当前这种 
        /// 2.属性注入    可以扩展
        /// 3.方法注入    可以扩展
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="iCompanyService"></param>
        public FirstController(IConfiguration configuration, ICompanyService iCompanyService)
        {
            this._Configuration = configuration;
            this._ICompanyService = iCompanyService;
        }

        public IActionResult Index()
        {  
            //数据如何传输给视图；
            ViewBag.User1 = "User1";
            ViewData["User2"] = "User2";
            TempData["User3"] = "User3";  //TempData实现就是基于Sesssion
            object User4 = "String"; //只能说对象，不能说string类型
            //return View(User4);

            //以上的传值方式都只能在当前Action对应的视图中共展示结果；
            //能不能做到？跳转也保存结果呢？     
            //ViewBag/ViewData不能，TempData与Session可以
            //return RedirectToAction("About");

            // Session: 
            //HttpContext.Session["User4"] = "猫九"; 
            HttpContext.Session.SetString("User5", "猫"); //注意：Session的使用需要配置； 
            //ViewBag.port = _Configuration["port"];
            return RedirectToAction("About");
        }

        public IActionResult About()
        {
            int iRsult = _ICompanyService.GetId(123);
            ViewBag.iResult = iRsult;
            return View();
        }
    }
}
