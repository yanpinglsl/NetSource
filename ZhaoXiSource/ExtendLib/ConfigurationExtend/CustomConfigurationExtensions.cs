using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.LogExtend
{
    /// <summary>
    /// 做个扩展
    /// </summary>
    public static class CustomConfigurationExtensions
    {
        public static IConfigurationBuilder AddCustomConfiguration(
            this IConfigurationBuilder builder, Action<CustomConfigurationOption> optionsAction)
        {
            return builder.Add(new CustomConfigurationSource(optionsAction));//配置个数据源
        }
    }
}
