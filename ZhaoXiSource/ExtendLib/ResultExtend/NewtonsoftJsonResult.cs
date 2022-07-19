using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ResultExtend
{
    /// <summary>
    /// 换一个类型使用---没有用Executor
    /// </summary>
    public class NewtonsoftJsonResult : ActionResult
    {
        private object _data;

        public NewtonsoftJsonResult(object data)
        {
            _data = data;
        }
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            Console.WriteLine($"This is {nameof(NewtonsoftJsonResult)} 序列化");
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            string result = JsonConvert.SerializeObject(this._data);
            await response.WriteAsync(result);
        }
    }
}
