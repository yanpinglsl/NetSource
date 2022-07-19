using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ControllerExtend
{
    /// <summary>
    /// 这个是控制器实例化的点
    /// 特性需标记InjectionPropertyAttribute的属性
    /// 需要IOC注册
    /// </summary>
    public class CustomControllerFactory : IControllerFactory
    {
        /// <summary>
        /// 跟CustomControllerActivator一模一样
        /// 默认实现，都是调用controllerActivator
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object CreateController(ControllerContext context)
        {
            Console.WriteLine($"This is {nameof(CustomControllerFactory)}.CreateController");
            var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
            var controllerInstance = context.HttpContext.RequestServices.GetService(controllerType);
            //这里也可以扩展属性注入，甚至AOP
            return controllerInstance;
        }

        /// <summary>
        /// 跟CustomControllerActivator一模一样
        /// 默认实现，都是调用controllerActivator
        /// </summary>
        /// <param name="context"></param>
        /// <param name="controller"></param>
        public void ReleaseController(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            var disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
