using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ResultExtend
{
    /// <summary>
    /// 
    /// </summary>
    public class NameValueOutputFormatter : IOutputFormatter
    {
        /// <summary>
        /// 选择时---判断是否用这个
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            //写死CurrentUser
            if (context.ObjectType == typeof(CurrentUser) || context.Object is CurrentUser)
            {
                return true;
            }

            return false;
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //格式化成这种
            var resultValue = string.Join(" && ", context.Object.GetType().GetProperties().Select(p => $"{p.Name}:{p.GetValue(context.Object)}"));

            var response = context.HttpContext.Response;
            response.ContentType = "text/plain; charset=utf-8";
            return response.WriteAsync(resultValue, Encoding.UTF8);
        }
    }
}
