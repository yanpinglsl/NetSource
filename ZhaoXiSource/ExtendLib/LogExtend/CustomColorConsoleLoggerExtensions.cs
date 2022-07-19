using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.LogExtend
{
    /// <summary>
    /// 扩展个方法，让ILoggerFactory去AddProvider
    /// </summary>
    public static class CustomColorConsoleLoggerExtensions
    {
        public static ILoggerFactory AddColorConsoleLogger(this ILoggerFactory loggerFactory, CustomColorConsoleLoggerConfiguration config)
        {
            loggerFactory.AddProvider(new CustomColorConsoleLoggerProvider(config));
            return loggerFactory;
        }
        public static ILoggerFactory AddColorConsoleLogger(this ILoggerFactory loggerFactory)
        {
            var config = new CustomColorConsoleLoggerConfiguration();
            return loggerFactory.AddColorConsoleLogger(config);
        }
        public static ILoggerFactory AddColorConsoleLogger(this ILoggerFactory loggerFactory, Action<CustomColorConsoleLoggerConfiguration> configure)
        {
            var config = new CustomColorConsoleLoggerConfiguration();
            configure(config);
            return loggerFactory.AddColorConsoleLogger(config);
        }
    }
}
