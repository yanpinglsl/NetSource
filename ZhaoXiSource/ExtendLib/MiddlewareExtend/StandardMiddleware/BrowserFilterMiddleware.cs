using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.StandardMiddleware
{
    public class BrowserFilterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IBrowserCheck _iBrowserCheck;
        private readonly BrowserFilterOptions _BrowserFilterOptions;

        /// <summary>
        /// IOptions<BrowserFilterOptions> options 有2个来源
        /// 既可以Use的时候去直接传递
        /// 也可以Add是Configure,这里再获取
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="browserCheck"></param>
        /// <param name="options"></param>

        public BrowserFilterMiddleware(RequestDelegate next, ILogger<BrowserFilterMiddleware> logger, IBrowserCheck browserCheck, IOptions<BrowserFilterOptions> options)
        {
            this._next = next;
            this._logger = logger;
            this._iBrowserCheck = browserCheck;
            this._BrowserFilterOptions = options.Value;
        }

        ///// <summary>
        ///// 玩法3一起生效---先Use为准，再叠加Add的委托
        ///// 这不是框架的推荐写法，仅用于技术研究和融合应用，需仔细验证
        ///// </summary>
        ///// <param name="next"></param>
        ///// <param name="logger"></param>
        ///// <param name="browserCheck"></param>
        ///// <param name="options"></param>
        //public BrowserFilterMiddleware(RequestDelegate next, ILogger<BrowserFilterMiddleware> logger, IBrowserCheck browserCheck, IConfigureOptions<BrowserFilterOptions> configureNamedOptions, IOptions<BrowserFilterOptions> options)
        //{
        //    this._next = next;
        //    this._logger = logger;
        //    this._iBrowserCheck = browserCheck;
        //    this._BrowserFilterOptions = options.Value;

        //    configureNamedOptions.Configure(options.Value);//玩法3一起生效---先Use为准，再叠加Add的委托
        //}

        /// <summary>
        /// 方法名字Invoke或者InvokeAsync
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var result = this._iBrowserCheck.CheckBrowser(context, this._BrowserFilterOptions);
            if (!result.Item1)
            {
                Console.WriteLine($"{nameof(BrowserFilterMiddleware)} {result.Item2}");
                await context.Response.WriteAsync($"{nameof(BrowserFilterMiddleware)} {result.Item2}");
            }
            else
            {
                Console.WriteLine($"{nameof(BrowserFilterMiddleware)} ok");
                await _next(context);
            }
        }


    }
}
