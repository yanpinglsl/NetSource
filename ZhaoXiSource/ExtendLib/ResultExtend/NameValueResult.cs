using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ResultExtend
{
    /// <summary>
    /// 格式化对象为自定义Name:Value && Name:Value && Name:Value 格式
    /// </summary>
    public class NameValueResult : ActionResult
    {
        public NameValueResult(object content)
        {
            Content = content;
        }
        public object Content { get; set; }
        public string ContentType { get; set; }
        public int? StatusCode { get; set; }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<NameValueResult>>();//容器获取executor
            await executor.ExecuteAsync(context, this);
        }
    }
    /// <summary>
    /// Executor
    /// </summary>
    public class NameValueResultExecutor : IActionResultExecutor<NameValueResult>
    {
        private const string DefaultContentType = "text/plain; charset=utf-8";
        private readonly IHttpResponseStreamWriterFactory _httpResponseStreamWriterFactory;

        public NameValueResultExecutor(IHttpResponseStreamWriterFactory httpResponseStreamWriterFactory)
        {
            _httpResponseStreamWriterFactory = httpResponseStreamWriterFactory;
        }
        /// <summary>
        /// 格式化器
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string FormatToString(object content)
        {
            return string.Join(" && ", content.GetType().GetProperties().Select(p => $"{p.Name}:{p.GetValue(content)}"));
        }

        /// <inheritdoc />
        public virtual async Task ExecuteAsync(ActionContext context, NameValueResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var response = context.HttpContext.Response;
            Encoding contentTypeEncoding = Encoding.UTF8;

            response.ContentType = DefaultContentType;
            if (result.StatusCode != null)
            {
                response.StatusCode = result.StatusCode.Value;
            }
            string content = FormatToString(result.Content);
            if (result.Content != null)
            {
                response.ContentLength = contentTypeEncoding.GetByteCount(content);
                using (var textWriter = _httpResponseStreamWriterFactory.CreateWriter(response.Body, contentTypeEncoding))
                {
                    await textWriter.WriteAsync(content);
                    await textWriter.FlushAsync();
                }
            }
        }
    }
}
