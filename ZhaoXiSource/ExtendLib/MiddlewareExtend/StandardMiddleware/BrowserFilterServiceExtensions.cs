using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.StandardMiddleware
{
    /// <summary>
    /// 集中注册内部映射，可以多个
    /// </summary>
    public static class BrowserFilterServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBrowserFilter(this IServiceCollection services)
        {
            return services.AddSingleton<IBrowserCheck, BrowserCheckService>();
        }

        /// <summary>
        /// 直接用Option的模式去初始化
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddBrowserFilter(this IServiceCollection services, Action<BrowserFilterOptions> configure)
        {
            services.Configure(configure);//这个是之前讲的Options,只是配置，但是生效是在访问Value属性时
            return services.AddBrowserFilter();
        }
    }
}
