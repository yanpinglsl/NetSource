using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOPFilterApp.Filter
{
    public class AuthonizationFilter : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// 过滤器中没有构造函数
        /// A.作用于全局：在ConfigureService中的AddMvc方法中进行注入，有两种写法，如：o.Filters.Add(typeof(AuthorizeFilter)); 或 o.Filters.Add(new AuthorizeFilter());
        /// B.作用于Controller或Action： 直接以特性的形式作用于Controller或Action即可，与Asp.Net中相同。
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //1. 获取区域、控制器、Action的名称
            ////必须在区域里的控制器上加个特性[Area("")]才能获取
            //var areaName = context.ActionDescriptor.RouteValues["area"] == null ? "" : context.ActionDescriptor.RouteValues["area"].ToString();
            var controllerName = context.ActionDescriptor.RouteValues["controller"] == null ? "" : context.ActionDescriptor.RouteValues["controller"].ToString();
            var actionName = context.ActionDescriptor.RouteValues["action"] == null ? "" : context.ActionDescriptor.RouteValues["action"].ToString();

            Console.WriteLine($"ControllerName:{controllerName}");
            Console.WriteLine($"ActionName:{actionName}");
            //下面的方式也能获取控制器和action的名称
            //var controllerName = context.RouteData.Values["controller"].ToString();
            //var actionName = context.RouteData.Values["action"].ToString();


            //这里可以做复杂的权限控制操作
            Console.WriteLine("OnAuthorization");
        }

    }
}
