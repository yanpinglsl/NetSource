using AOPFilterApp.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AOPFilterApp.Controllers
{
    //[AuthonizationFilter]
    [CustomActionOrderFilterAttribute(Order = 10)]
    public class FilterController : Controller
    {
        #region Identity
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<FilterController> _logger;

        public FilterController(IConfiguration configuration
            , ILogger<FilterController> logger)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
        }
        #endregion

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/Filter/Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Index LogWarning");

            return View();
        }

        #region Test
        [AuthonizationFilter]
        public IActionResult Auth1()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Auth1 LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        //[AuthonizationFilter2] //会报错
        //[ServiceFilter(typeof(AuthonizationFilter2))]
        [TypeFilter(typeof(AuthonizationFilter2))]
        public IActionResult Auth2()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Auth2 LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        [AuthonizationFilter3]
        public IActionResult Auth3()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Auth2 LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }

        #endregion
        #region Filter排序
        /// <summary>
        /// http://localhost:5726/Filter/Info
        /// 1 全局---控制器---Action  
        /// 2 Order默认0，从小到大执行  可以负数
        /// </summary>
        /// <returns></returns>
        //[TypeFilter(typeof(CustomActionOrderFilterAttribute), Order = 10, IsReusable = false)]
        [CustomActionOrderFilterAttribute(Order = 1)]
        [CustomActionOrderFilterAttribute(Remark = "Before")]
        [CustomActionOrderFilterAttribute(Remark = "After")]//我后执行
        //[CustomActionCacheFilterAttribute(Order = -1)]
        ////[IResourceFilter]
        public IActionResult Info()
        {
            this._logger.LogWarning($"This is {nameof(FilterController)}-Info LogWarning");

            base.ViewBag.Now = DateTime.Now;
            Thread.Sleep(2000);
            return View();
        }
        #endregion
    }
}
