using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExtendLib.ResultExtend
{
    /// <summary>
    /// Result的ExecuteResultAsync 不就是完成响应吗
    /// </summary>
    public class XmlResult : ActionResult
    {
        private object _data;

        public XmlResult(object data)
        {
            _data = data;
        }
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            response.ContentType = "text/xml";
            string result = _data.XMLSerialize();
            await response.WriteAsync(result);
        }
    }
    public static class XMLExtension
    {
        public static string XMLSerialize<T>(this T t)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(t.GetType());
                xz.Serialize(sw, t);
                return sw.ToString();
            }
        }
    }
}
