using Common.Util;
using ExtendLib;
using ExtendLib.ControllerExtend;
using ExtendLib.ResultExtend;
using IOCTestInterfaceLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    /// <summary>
    /// 控制器-Action-Result
    /// </summary>
    //[CORSFilter]
    //[CustomControllerModelConvention]
    public class CARController : Controller
    {
        #region Identity
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<CARController> _logger;


        public CARController(IConfiguration configuration
            , ILogger<CARController> logger
            , IHttpContextAccessor httpContextAccessor)
        {
            this._iConfiguration = configuration;
            this._logger = logger;
        }

        //[InjectionProperty]标记，用于属性注入
        [InjectionProperty]
        public ITestServiceE _iTestServiceEPropertyInject
        {
            get
            {
                return _iTestServiceEPropertyInject_Value;
            }
            set
            {
                Console.WriteLine($"This is {nameof(_iTestServiceEPropertyInject)} Init");
                _iTestServiceEPropertyInject_Value = value;
            }
        }
        private ITestServiceE _iTestServiceEPropertyInject_Value;
        #endregion

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/CAR/Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            this._logger.LogWarning($"This is {nameof(CARController)} Index");
            return View();
        }

        /// <summary>
        /// http://localhost:5726/CAR/ArgumentBinder?name=Eleven
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult ArgumentBinder(string name)
        {
            this._logger.LogWarning($"This is {nameof(CARController)} {nameof(ArgumentBinder)},name={name}");

            return new JsonResult(
                new AjaxResult()
                {
                    Value = $"name={name}",
                    Result = true
                }
                );
        }

        ///// <summary>
        ///// http://localhost:5726/CAR/ArgumentService
        ///// </summary>
        ///// <param name="testServiceA"></param>
        ///// <returns></returns>
        //public CurrentUser ArgumentService([FromServices] ITestServiceA testServiceA)
        //{
        //    this._logger.LogWarning($"This is {nameof(CARController)} {nameof(ArgumentService)}");
        //    testServiceA.Show();
        //    return new CurrentUser()
        //    {
        //        Id = 123,
        //        Account = "Administrator",
        //        Name = testServiceA.GetHashCode().ToString()
        //    };
        //}



        /// <summary>
        /// http://localhost:5726/CAR/Result?name=Eleven1
        /// http://localhost:5726/CAR/Result?name=Eleven2
        /// http://localhost:5726/CAR/Result?name=Eleven3
        /// http://localhost:5726/CAR/Result?name=Eleven4
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult Result(string name)
        {
            this._logger.LogWarning($"This is {nameof(CARController)} {nameof(Result)},name={name}");
            switch (name.ToLower())
            {
                case "eleven1":
                    return new JsonResult(
                        new AjaxResult()
                        {
                            Value = $"name={name}",
                            Result = true
                        });
                case "eleven2":
                    return new NewtonsoftJsonResult(
                        new AjaxResult()
                        {
                            Value = $"name={name}",
                            Result = true
                        });
                case "eleven3":
                    return new XmlResult(
                        new AjaxResult()
                        {
                            Value = $"name={name}",
                            Result = true
                        });
                case "eleven4":
                    return new NameValueResult(
                        new AjaxResult()
                        {
                            Value = $"name={name}",
                            Result = true
                        });
                default:
                    return new JsonResult(
                        new AjaxResult()
                        {
                            Value = $"name={name}",
                            Result = true
                        });
            }
        }

        /// <summary>
        /// http://localhost:5726/CAR/StringResult?name=Eleven
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CurrentUser StringResult(string name)
        {
            this._logger.LogWarning($"This is {nameof(CARController)} {nameof(StringResult)},name={name}");
            return new CurrentUser()
            {
                Id = 123,
                Account = "Administrator",
                Name = name
            };
        }

        ///// <summary>
        ///// 输出格式化问题
        ///// http://localhost:5726/CAR/VoidResult?name=seven
        ///// http://localhost:5726/CAR/VoidResult?name=Eleven
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public void VoidResult(string name)
        //{
        //    this._logger.LogWarning($"This is {nameof(CARController)} {nameof(VoidResult)},name={name}");
        //    if ("Eleven".Equals(name, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        return;
        //    }

        //    var user = new CurrentUser()
        //    {
        //        Id = 123,
        //        Account = "Administrator",
        //        Name = name
        //    };
        //    var resultValue = string.Join(" && ", user.GetType().GetProperties().Select(p => $"{p.Name}:{p.GetValue(user)}"));

        //    base.Response.ContentType = "text/plain; charset=utf-8";
        //    base.Response.WriteAsync(resultValue, Encoding.UTF8);
        //}

        ///// <summary>
        ///// http://localhost:5726/CAR/ViewShow
        ///// http://localhost:5726/CAR/ViewShow?style=m
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult ViewShow()
        //{
        //    List<string> viewList = new List<string>();
        //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetTypes().Count(t => t.Name.StartsWith("AspNetCore.Views_")) > 0))
        //    //view编辑后生成的都是以AspNetCore.Views_开头的，
        //    {
        //        viewList.Add($"dll-Name:{assembly.FullName}");
        //        foreach (var type in assembly.GetTypes())
        //        {
        //            viewList.Add($"type-Name:{type.FullName}");
        //        }
        //    }
        //    base.ViewBag.InfoList = viewList;
        //    return View();
        //}

        /// <summary>
        /// http://localhost:5726/CAR/ConventionController
        /// </summary>
        /// <returns></returns>
        public string ConventionController()
        {
            return $"CARController-ConventionController ";
        }
        ///// <summary>
        ///// http://localhost:5726/CAR/ConventionAction
        ///// </summary>
        ///// <returns></returns>
        //[CustomActionConventionAttribute]
        //public string ConventionAction()
        //{
        //    return $"CARController-ConventionAction";
        //}

        ///// <summary>
        ///// http://localhost:5726/CAR/ConventionParameter/123
        ///// </summary>
        ///// <returns></returns>
        //public string ConventionParameter([CustomParameterConventionAttribute] int id)
        //{
        //    return $"CARController-ConventionParameter {id}";
        //}
    }
}
