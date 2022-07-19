using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.LogExtend
{
    /// <summary>
    /// 驱动程序：为每个类都创建一个写日志的实例，保存在字典里面
    ///           其实也可以多种类型的，控制台、文本、等等
    /// </summary>
    public class CustomColorConsoleLoggerProvider : ILoggerProvider
    {
        private readonly CustomColorConsoleLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, CustomColorConsoleLogger> _loggers = new ConcurrentDictionary<string, CustomColorConsoleLogger>();

        public CustomColorConsoleLoggerProvider(CustomColorConsoleLoggerConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 为每个类都创建一个写日志的实例，保存在字典里面
        ///           其实也可以多种类型的，控制台、文本、等等
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new CustomColorConsoleLogger(name, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
