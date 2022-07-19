using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.SimpleExtend
{
    /// <summary>
    /// 1  实现IMiddleware，就不能有参数--而且还得IOC注册
    /// 2  SecondMiddleWare类型的初始化是请求来了之后才发生，跟FirstMiddleware不一样
    /// 3  如果响应请求时，才实例化，用完立即释放---这种就应该实现IMiddleware
    /// </summary>
    public class SecondMiddleWare : IMiddleware, IDisposable
    {
        private readonly ILogger _logger;

        public SecondMiddleWare(ILogger<SecondMiddleWare> logger)
        {
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            this._logger.LogWarning($"{nameof(SecondMiddleWare)},Hello World1!<br/>");

            await context.Response.WriteAsync($"{nameof(SecondMiddleWare)},Hello World1!<br/>");
            await next(context);
            await context.Response.WriteAsync($"{nameof(SecondMiddleWare)},Hello World2!<br/>");
        }

        public void Dispose()
        {
            Console.WriteLine("释放需要释放的资源");
        }
    }
}
