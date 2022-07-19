using ExtendLib.FilterExtend;
using IOCTestInterfaceLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
    public class CacheController : Controller
    {
        #region Identity
        private readonly IConfiguration _iConfiguration = null;
        private readonly ILogger<CacheController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITestServiceA _iTestServiceA;
        private readonly IMemoryCache _iMemoryCache;
        private readonly IDistributedCache _iCache;

        public CacheController(IConfiguration configuration,
            ILoggerFactory loggerFactory
            , ILogger<CacheController> logger
            , ITestServiceA testServiceA
            , IDistributedCache cache
            , IMemoryCache memoryCache
           )
        {
            this._iConfiguration = configuration;
            this._logger = logger;
            this._loggerFactory = loggerFactory;
            this._iTestServiceA = testServiceA;
            //base.HttpContext.RequestServices.GetService<IA>();
            this._iMemoryCache = memoryCache;
            this._iCache = cache;
        }
        #endregion

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/Cache/Index
        /// </summary>
        /// <returns></returns>
        //[ResponseCache(Duration = 30)]
        //[ResponseCache(CacheProfileName = "default1")]
        [CustomCacheResultFilterAttribute(Duration = 600)]
        public IActionResult Index()
        {
            this._logger.LogWarning($"This is {nameof(CacheController)} Index");

            //base.HttpContext.Response.Headers["Cache-Control"] = "public,max-age=600";

            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            base.ViewBag.Url = $"{base.Request.Scheme}://{base.Request.Host}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{this._iConfiguration["port"]}";//应用程序监听地址

            return View();
        }
        [ResponseCache(Duration = 600)]
        public IActionResult List(int page = 0)
        {
            return Content(page.ToString());
        }

        /// <summary>
        /// 反向代理
        /// http://localhost:5726/Cache/ReverseProxy
        /// http://localhost:8080/Cache/ReverseProxy
        /// </summary>
        /// <returns></returns>
        public IActionResult ReverseProxy()
        {
            this._logger.LogWarning($"This is {nameof(CacheController)} Static");

            //base.HttpContext.Response.Headers["Cache-Control"] = "public,max-age=600";

            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            base.ViewBag.Url = $"{base.Request.Scheme}://{base.Request.Host}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{this._iConfiguration["port"]}";//应用程序监听地址

            return View();
        }

        /// <summary>
        /// 做静态化
        /// http://localhost:5726/item/123.html
        /// http://localhost:8080/item/123.html
        /// </summary>
        /// <returns></returns>
        [Route("/item/{id:int}.html")]
        public IActionResult Static(int id)
        {
            this._logger.LogWarning($"This is {nameof(CacheController)} Static {id}");

            //base.HttpContext.Response.Headers["Cache-Control"] = "public,max-age=600";

            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            base.ViewBag.Url = $"{base.Request.Scheme}://{base.Request.Host}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{this._iConfiguration["port"]}";//应用程序监听地址

            return View();
        }


        /// <summary>
        /// 中间件缓存
        /// http://localhost:5726/Cache/MiddlewareCache
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 600)]
        public IActionResult MiddlewareCache()
        {
            this._logger.LogWarning($"This is {nameof(CacheController)} MiddlewareCache");

            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            base.ViewBag.Url = $"{base.Request.Scheme}://{base.Request.Host}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{this._iConfiguration["port"]}";//应用程序监听地址

            return View();
        }

        /// <summary>
        /// 中间件缓存
        /// http://localhost:5726/Cache/Filter
        /// </summary>
        /// <returns></returns>
        [CustomDictionaryResourceFilterAttribute]
        public IActionResult Filter()
        {
            this._logger.LogWarning($"This is {nameof(CacheController)} Filter");

            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            base.ViewBag.Url = $"{base.Request.Scheme}://{base.Request.Host}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{this._iConfiguration["port"]}";//应用程序监听地址

            return View();
        }

        /// <summary>
        /// http://localhost:5726/Cache/BackendCache
        /// </summary>
        /// <returns></returns>
        public IActionResult BackendCache()
        {
            this._logger.LogWarning($"This is {nameof(CacheController)} BackendCache");

            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            base.ViewBag.Url = $"{base.Request.Scheme}://{base.Request.Host}/  ";//浏览器地址
            base.ViewBag.InternalUrl = $"{base.Request.Scheme}://{this._iConfiguration["port"]}";//应用程序监听地址

            string key = $"CacheController-BackendCache";
            #region MemoryCache
            {
                if (this._iMemoryCache.TryGetValue<string>(key, out string time))
                {
                }
                else
                {
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                    this._iMemoryCache.Set(key, time, TimeSpan.FromSeconds(120));
                }
                base.ViewBag.MemoryCacheNow = time;
            }
            #endregion

            #region 分布式缓存
            {
                string time = this._iCache.GetString(key);
                if (!string.IsNullOrWhiteSpace(time))
                {

                }
                else
                {
                    time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                    this._iCache.SetString(key, time, new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120)
                    });
                }
                base.ViewBag.DistributedCacheNow = time;
            }
            #endregion

            return View();
        }
    }
}
