using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend
{
    /// <summary>
    /// 中间件--Invoke---Next---放在前面
    /// 
    /// 能保证，如果是指定站点访问图片，正常展示
    /// 如果是单独请求(爬虫)，或者其他站点(盗链)，就展示404
    /// 这个就是防盗链！
    /// </summary>
    public class RefuseStealingMiddleWare
    {
        private readonly RequestDelegate _next;

        public RefuseStealingMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string url = context.Request.Path.Value;
            if (!url.Contains(".jpg"))
            {
                await _next(context);//走正常流程，啥事儿不干
            }
            else
            {
                string urlReferrer = context.Request.Headers["Referer"];
                if (string.IsNullOrWhiteSpace(urlReferrer))//直接访问
                {
                    await this.SetForbiddenImage(context);//返回404图片
                }
                else if (!urlReferrer.Contains("localhost"))//非当前域名
                {
                    await this.SetForbiddenImage(context);//返回404图片
                }
                else
                {
                    await _next(context);//走正常流程
                }
            }
        }
        /// <summary>
        /// 设置拒绝图片
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task SetForbiddenImage(HttpContext context)
        {
            string defaultImagePath = "wwwroot/image/Forbidden.jpg";
            string path = Path.Combine(Directory.GetCurrentDirectory(), defaultImagePath);

            FileStream fs = File.OpenRead(path);
            byte[] bytes = new byte[fs.Length];
            //context.Response.Headers["ContentType"] = "";
            await fs.ReadAsync(bytes, 0, bytes.Length);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }

    /// <summary>
    /// 扩展个注册
    /// </summary>
    public static class RefuseStealingMiddleWareExtensions
    {
        public static void UseRefuseStealing(this IApplicationBuilder app)
        {
            app.UseMiddleware<RefuseStealingMiddleWare>();
        }
    }
}
