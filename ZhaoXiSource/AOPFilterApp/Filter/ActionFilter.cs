using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOPFilterApp.Filter
{
    public class ActionFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //执行完成....
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //执行中...
        }
    }
    /// <summary>
    /// 测试操作过滤器，自定义Order
    /// （测试作用于Action）
    /// </summary>
    public class ActionOrderFilter : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }

    /// <summary>
    /// 测试操作过滤器，自定义Order
    /// （测试作用于Controller）
    /// </summary>
    public class ActionOrderFilterController : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }

    /// <summary>
    /// 测试操作过滤器，自定义Order
    /// （测试作用于全局）
    /// </summary>
    public class ActionOrderFilterGlobal : Attribute, IActionFilter, IOrderedFilter
    {
        public int Order { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
