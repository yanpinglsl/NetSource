using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOPFilterApp.Filter
{
    public class ResourceFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // 执行完后的操作
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // 执行中的过滤器管道
        }
    }
}
