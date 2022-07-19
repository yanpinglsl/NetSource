using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExtendLib.ControllerExtend.ControlTestDemo
{
    /// <summary>
    /// http://localhost:5726/part/index
    /// </summary>
    [Controller]//要么标记特性  要么Controller结尾
    public class PartController : Microsoft.AspNetCore.Mvc.Controller
    {
        #region Identity
        private readonly ILogger<PartController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        public PartController(ILogger<PartController> logger,
            ILoggerFactory loggerFactory)
        {
            this._logger = logger;
            this._loggerFactory = loggerFactory;
        }
        #endregion

        public IActionResult Index()
        {
            this._logger.LogWarning($"This is Zhaoxi.AgileFramework.WebCore.ControllerExtend.PartDemo.{nameof(PartController)}");

            base.ViewData["Info"] = $"{nameof(PartController)}.Info"; 

            return View();
        }
    }
}