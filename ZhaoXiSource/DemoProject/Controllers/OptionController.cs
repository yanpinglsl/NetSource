using DemoProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    public class OptionController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<OptionController> _logger;

        private IOptions<EmailOption> _optionsDefault;//直接单例，不支持数据变化，性能高
        private IOptionsMonitor<EmailOption> _optionsMonitor;//支持数据修改，靠的是监听文件更新(onchange)数据
        private IOptionsSnapshot<EmailOption> _optionsSnapshot;//一次请求数据不变的，但是不同请求可以不同的，每次生成
        private EmailOption _emailOption;//使用解除Option绑定后的“原始”配置对象

        public OptionController(IOptions<EmailOption> options
            , IOptionsMonitor<EmailOption> optionsMonitor
            , IOptionsSnapshot<EmailOption> optionsSnapshot
            , IConfiguration configuration
            , EmailOption emailOption
            , ILogger<OptionController> logger)
        {
            this._optionsDefault = options;
            this._optionsMonitor = optionsMonitor;
            this._optionsSnapshot = optionsSnapshot;
            this._emailOption = emailOption;

            this._iConfiguration = configuration;
            this._logger = logger;
        }

        /// <summary>
        /// http://localhost:5726/Option
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            this._logger.LogWarning($"This is {nameof(OptionController)} Index");

            //可以直接访问配置文件---但是使用的地方得知道配置文件路径
            //就是IOC注入参数--很僵化
            base.ViewBag.defaultEmailOption = this._optionsDefault.Value;

            base.ViewBag.defaultEmailOption1 = _optionsMonitor.CurrentValue;//_optionsMonitor.Get(Microsoft.Extensions.Options.Options.DefaultName);
            base.ViewBag.fromMemoryEmailOption1 = _optionsMonitor.Get("FromMemory");
            base.ViewBag.fromConfigurationEmailOption1 = _optionsMonitor.Get("FromConfiguration");
            base.ViewBag.fromConfigurationEmailOptionNew = _optionsMonitor.Get("FromConfigurationNew");//直接目录修改配置文件，就能看到变化

            base.ViewBag.defaultEmailOption2 = _optionsSnapshot.Value;//_optionsSnapshot.Get(Microsoft.Extensions.Options.Options.DefaultName);
            base.ViewBag.fromMemoryEmailOption2 = _optionsSnapshot.Get("FromMemory");
            base.ViewBag.fromConfigurationEmailOption2 = _optionsSnapshot.Get("FromConfiguration");

            return View();
        }

    }
}
