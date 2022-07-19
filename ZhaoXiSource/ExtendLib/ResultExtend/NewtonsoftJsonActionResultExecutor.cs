using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ResultExtend
{
    /// <summary>
    /// 替换序列化器Executor--可以加点额外扩展
    /// </summary>
    public class NewtonsoftJsonActionResultExecutor : IActionResultExecutor<JsonResult>
    {
        public virtual async Task ExecuteAsync(ActionContext context, JsonResult result)
        {
            Console.WriteLine("***************This is  NewtonsoftJsonActionResultExecutor 序列化");

            var response = context.HttpContext.Response;

            response.ContentType = "application/json"; ;
            var value = result.Value;
            var objectType = value?.GetType() ?? typeof(object);

            // Keep this code in sync with SystemTextJsonOutputFormatter
            var responseStream = response.Body;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            await response.WriteAsync(json, Encoding.UTF8);
        }
    }
}
