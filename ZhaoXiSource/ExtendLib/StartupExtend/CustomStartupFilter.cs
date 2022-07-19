using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.StartupExtend
{
    /// <summary>USeWhen
    /// 发生在Run的时候，执行Startup类的Configure方法之前
    /// 
    /// 需要注册到IOC容器去
    /// 
    /// 头尾拦截加东西----缓存初始化、配置文件检测提前报错、黑白名单组件
    /// </summary>
    public class CustomStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            //执行会出错？？？
            //因为中间件中不能使用context.Response,因为在调用最后一层代码时，响应长度已确定，如果修改则报错
            //return new Action<IApplicationBuilder>(
            // app =>
            //{
            //    app.Use(next =>
            //    {
            //        Console.WriteLine($"This is {nameof(CustomStartupFilter)} middleware 1");
            //        return new RequestDelegate(
            //            async (context) =>
            //            {
            //                 context.Response.WriteAsync($"This is {nameof(CustomStartupFilter)} Hello World 1 start");
            //                await next.Invoke(context);
            //                 context.Response.WriteAsync($"This is {nameof(CustomStartupFilter)} Hello World 1   end");
            //                 Task.Run(() => Console.WriteLine($"{nameof(CustomStartupFilter)} 12345678797989"));
            //            });
            //    });
            //    next.Invoke(app);
            //}
            //);
            //问题：为什么该处理会被多次调用
            return new Action<IApplicationBuilder>(
             app =>
            {
                app.Use(next =>
                {
                    Console.WriteLine($"This is {nameof(CustomStartupFilter)} middleware 1");
                    return new RequestDelegate(
                        async (context) =>
                        {
                            Console.WriteLine($"This is {nameof(CustomStartupFilter)} Hello World 1 start");
                            await next.Invoke(context);
                            Console.WriteLine($"This is {nameof(CustomStartupFilter)} Hello World 1   end");
                            await Task.Run(() => Console.WriteLine($"{nameof(CustomStartupFilter)} 12345678797989"));
                        });
                });
                next.Invoke(app); 
            }
            );
        }
    }
}
