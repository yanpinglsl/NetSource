using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.StandardMiddleware
{
    /// <summary>
    /// 扩展注册---3种方式
    /// </summary>
    public static class BrowserFilterMiddlewareExtensions
    {
        /// <summary>
        /// 2个地方同时要注册，所以这个不行
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            //BrowserFilterOptions browserFilterOptions = new BrowserFilterOptions();

            return app.UseMiddleware<BrowserFilterMiddleware>();
            //没有Option，就依靠Add的设置
        }

        /// <summary>
        /// 不能提供options了，IOC时已初始化？？？
        /// </summary>
        /// <param name="app"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app, BrowserFilterOptions options)
        {
            //return app.UseMiddleware<BrowserFilterMiddleware>(options);//这样传递是错的，因为要的是IOptions<BrowserFilterOptions>不能提供options了，IOC时已初始化？？？

            #region 
            return app.UseMiddleware<BrowserFilterMiddleware>(Options.Create(options));//IOptions<TOptions>---OptionsWrapper----直接写入对象，直接覆盖数据-----上面那个，则是不管，以Add为准(IOption则是IOC注入的)
            #endregion
        }

        /// <summary>
        /// 这个不能用了---
        /// 1  不能直接初始化Option
        /// 2  也不能找到ServiceCollection去初始化了
        /// </summary>
        /// <param name="app"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        [Obsolete("请不要使用这个", true)]
        public static IApplicationBuilder UseBrowserFilter(this IApplicationBuilder app, Action<BrowserFilterOptions> optionsAction)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            BrowserFilterOptions browserFilterOptions = new BrowserFilterOptions();
            optionsAction.Invoke(browserFilterOptions);
            //return app.UseBrowserFilter(browserFilterOptions);
            return app.UseMiddleware<BrowserFilterMiddleware>(Options.Create(browserFilterOptions));
        }
    }
}
