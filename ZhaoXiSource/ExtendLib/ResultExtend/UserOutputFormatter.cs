using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ResultExtend
{
    public class UserOutputFormatter : TextOutputFormatter
    {
        public UserOutputFormatter()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add("text/User");
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ObjectType == typeof(CurrentUser) || context.Object is CurrentUser)
            {
                return base.CanWriteResult(context);
            }//返回值是个CurrentUser 就由我来格式化

            //context.HttpContext.Request //其实还能判断各种---主要是Header

            return false;
        }

        private static string FormatToString(object content)
        {
            return string.Join(" || ", content.GetType().GetProperties().Select(p => $"{p.Name}:{p.GetValue(content)}"));
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            var valueAsString = FormatToString(context.Object as CurrentUser);
            if (string.IsNullOrEmpty(valueAsString))
            {
                await Task.CompletedTask;
            }

            var response = context.HttpContext.Response;
            await response.WriteAsync(valueAsString, selectedEncoding);
        }
    }

}
