using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.SimpleExtend
{
    /// <summary>
    /// 多一层工厂
    /// </summary>
    public class SecondMiddleWareFactory : IMiddlewareFactory
    {
        private readonly IServiceProvider _iServiceProvider;
        private readonly ILogger _logger;

        public SecondMiddleWareFactory(IServiceProvider serviceProvider, ILogger<SecondMiddleWareFactory> logger)
        {
            this._iServiceProvider = serviceProvider;
            this._logger = logger;
        }

        public IMiddleware Create(Type middlewareType)
        {
            return (IMiddleware)this._iServiceProvider.GetService(middlewareType);
        }

        /// <summary>
        /// middleware响应时才生成，尽快释放
        /// </summary>
        /// <param name="middleware"></param>
        public void Release(IMiddleware middleware)
        {
            if (middleware != null)
            {
                (middleware as IDisposable)?.Dispose();
            }
        }
    }
}
