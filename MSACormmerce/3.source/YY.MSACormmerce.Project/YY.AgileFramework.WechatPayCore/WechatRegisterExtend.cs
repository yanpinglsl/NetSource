using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.AgileFramework.WechatPayCore
{
    public static class WechatRegisterExtend
    {
        public static void AddWechatPay(this IServiceCollection services)
        {
            services.AddTransient<IWxPayConfig, WxPayConfig>();
            services.AddTransient<PayHelper>();
            services.AddTransient<WxPayApi>();
            services.AddTransient<WxPayHttpService>();


        }
    }
}
