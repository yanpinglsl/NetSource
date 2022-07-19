using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.SimpleExtend
{
    public class ThreeMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly string _Message;

        public ThreeMiddleWare(RequestDelegate next, ILogger<ThreeMiddleWare> logger, string message)
        {
            this._next = next;
            this._logger = logger;
            this._Message = message;
        }
        /// <summary>
        /// 1 方法名字Invoke或者InvokeAsync
        /// 2 返回类型必须是Task
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"{nameof(ThreeMiddleWare)}---{this._Message}");
            if (!context.Request.Path.Value.Contains("Eleven"))
            {
                await context.Response.WriteAsync($"{nameof(ThreeMiddleWare)}This is End<br/>");
            }
            else
            {
                await context.Response.WriteAsync($"{nameof(ThreeMiddleWare)},Hello World ThreeMiddleWare!<br/>");
                await _next(context);
                await context.Response.WriteAsync($"{nameof(ThreeMiddleWare)},Hello World ThreeMiddleWare!<br/>");
            }
        }
    }
}
