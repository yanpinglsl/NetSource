using DemoProject.Utility;
using IOCTestInterfaceLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class IOCController : Controller
    {
        #region Identity
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<IOCController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITestServiceA _iTestServiceA;
        private readonly ITestServiceB _iTestServiceB;
        private readonly ITestServiceC _iTestServiceC;
        private readonly ITestServiceD _iTestServiceD;
        private readonly ITestServiceE _iTestServiceE;
        private readonly IA _iA;
        private readonly IB _iB;
        private readonly IC _iC;
        private readonly IServiceProvider _iServiceProvider;


        public IOCController(IConfiguration configuration,
            ILoggerFactory loggerFactory
            , ILogger<IOCController> logger
            , ITestServiceA testServiceA
            , ITestServiceB testServiceB
            , ITestServiceC testServiceC
            , ITestServiceD testServiceD
            , ITestServiceE testServiceE
            , IA aInstance
            , IB bInstance
            , IC cInstance
            , IServiceProvider serviceProvider)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
            this._loggerFactory = loggerFactory;
            this._iTestServiceA = testServiceA;
            this._iTestServiceB = testServiceB;
            this._iTestServiceC = testServiceC;
            this._iTestServiceD = testServiceD;
            this._iTestServiceE = testServiceE;
            this._iA = aInstance;
            this._iB = bInstance;
            this._iC = cInstance;
            this._iServiceProvider = serviceProvider;

            //base.HttpContext.RequestServices.GetService<IA>();
        }
        #endregion

        /// <summary>
        /// 纯属测试 毫无意义
        /// </summary>
        private static ITestServiceC _iTestServiceCStatic = null;
        private static ITestServiceB _iTestServiceBStatic = null;

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/IOC/Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //ITestServiceA testServiceA = new TestServiceA();
            this._logger.LogWarning($"This is {nameof(IOCController)} Index");
            this._iTestServiceA.Show();
            this._iTestServiceB.Show();
            this._iTestServiceC.Show();
            this._iTestServiceD.Show();
            this._iTestServiceE.Show();
            this._iA.Show(1, "hahahah");
            this._iB.Show(2, "BBBBBBBBBBBB");

            //this._iServiceProvider.CreateScope().ServiceProvider.GetService//创建新的provider

            Console.WriteLine($"*****************************************");//T/F
            //#region 单例
            //{
            //    var b = this._iServiceProvider.GetService<ITestServiceB>();
            //    Console.WriteLine($"bb {object.ReferenceEquals(this._iTestServiceB, b)}");//T/F

            //    if (_iTestServiceBStatic == null)
            //    {
            //        _iTestServiceBStatic = _iTestServiceB;
            //    }
            //    else
            //    {
            //        Console.WriteLine($"B&B：{object.ReferenceEquals(this._iTestServiceB, _iTestServiceBStatic)}");//两次不同的请求 
            //    }
            //}
            //#endregion

            //#region 作用域周期
            //{
            //    var c = this._iServiceProvider.GetService<ITestServiceC>();
            //    Console.WriteLine($"cc {object.ReferenceEquals(this._iTestServiceC, c)}");//T/F

            //    var c2 = this._iServiceProvider.CreateScope().ServiceProvider.GetService<ITestServiceC>();
            //    Console.WriteLine($"cc2 {object.ReferenceEquals(c, c2)}");//T/F

            //    if (_iTestServiceCStatic == null)
            //    {
            //        _iTestServiceCStatic = _iTestServiceC;
            //    }
            //    else
            //    {
            //        Console.WriteLine($"C&C {object.ReferenceEquals(this._iTestServiceC, _iTestServiceCStatic)}");//两次不同的请求
            //    }
            //}
            //#endregion

            //Console.WriteLine($"_iTestServiceEPropertyInject is null ? {_iTestServiceEPropertyInject is null}");

            return View();
        }


        /// <summary>
        /// autofac-AOP
        /// </summary>
        /// <returns></returns>
        public IActionResult AOP()
        {
            this._logger.LogWarning($"This is {nameof(AOP)} Index");

            //var a = this._iServiceProvider.GetService<IA>();
            //a.Show(123, "Eleven");

            this._iC.Show(123, "Eleven");
            return View();
        }

    }
}
