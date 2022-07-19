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
    /// </summary>
    public class CustomControllerActivator : IControllerActivator
    {
        public object Create(ControllerContext context)
        {
            Console.WriteLine($"This is {nameof(CustomControllerActivator)} Create");
            var controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();//控制器的类型
            var controllerInstance = context.HttpContext.RequestServices.GetService(controllerType);//用容器完成控制器的实例化--能构造函数注入

            //属性注入
            foreach (var prop in controllerType.GetProperties().Where(p => p.IsDefined(typeof(InjectionPropertyAttribute), true)))
            {
                var propValue = context.HttpContext.RequestServices.GetService(prop.PropertyType);
                prop.SetValue(controllerInstance, propValue);
            }
            //方法注入---
            //构造完控制器之后，给属性绑定个值，来自于http请求
            return controllerInstance;
        }

        public void Release(ControllerContext context, object controller)
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
