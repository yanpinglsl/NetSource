using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOPFilterApp.Filter
{
    public class ExecptionFilter : Attribute, IExceptionFilter
    {
        private ILogger<ExecptionFilter> _logger;
        //构造注入日志组件
        public ExecptionFilter(ILogger<ExecptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            //日志收集
            _logger.LogError(context.Exception, context?.Exception?.Message ?? "异常");
        }
    }
}
