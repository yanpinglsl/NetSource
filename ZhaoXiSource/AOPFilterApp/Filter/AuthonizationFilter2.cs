using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOPFilterApp.Filter
{
    /// <summary>
    /// 授权过滤器2（含构造函数）
    /// A.作用于全局：与上面情况一用法一样，直接在AddMvc方法中进行注入，即可以使用过滤器中的构造函数中注入的对象，不需要特殊处理。
    ///B.作用于Controller或Action：发现如果直接以特性的形式进行作用，会报错缺少参数，这个时候正式引入两个特别的内置类，来处理这个问题:
    ///① ServiceFilterAttribute：首先在控制器或action上这样用 [ServiceFilter(typeof(AuthorizeFilter2))], 
    ///     然后在 ConfigureService中对该类进行注册一下， 如： services.AddScoped<AuthorizeFilter2>();
    ///② TypeFilterAttribute： 在控制器或action上这样用 [TypeFilter(typeof(AuthorizeFilter2))] 即可，
    ///     如下面的Index，不需要再在ConfigureService中进行注册了， 相比上面的ServiceFilterAttribute更方便。
    /// </summary>
    public class AuthonizationFilter2 : Attribute, IAuthorizationFilter
    {
        private IConfiguration Configuration;

        public AuthonizationFilter2(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //1. 获取区域、控制器、Action的名称
            ////必须在区域里的控制器上加个特性[Area("")]才能获取
            //var areaName = context.ActionDescriptor.RouteValues["area"] == null ? "" : context.ActionDescriptor.RouteValues["area"].ToString();
            var controllerName = context.ActionDescriptor.RouteValues["controller"] == null ? "" : context.ActionDescriptor.RouteValues["controller"].ToString();
            var actionName = context.ActionDescriptor.RouteValues["action"] == null ? "" : context.ActionDescriptor.RouteValues["action"].ToString();
            Console.WriteLine($"ControllerName:{controllerName}");
            Console.WriteLine($"ActionName:{actionName}");
            //2. 测试构造函数注入内容的读取
            var myName = Configuration["myName"];
            Console.WriteLine($"Configuration:{myName}");
        }
    }
}
