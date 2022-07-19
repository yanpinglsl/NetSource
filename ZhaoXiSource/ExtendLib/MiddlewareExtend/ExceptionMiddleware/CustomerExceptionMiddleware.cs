using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.ExceptionMiddleware
{
    public class CustomerExceptionMiddleware
    {
        /// <summary>
        /// 委托
        /// </summary>
        private readonly RequestDelegate _next;

        public CustomerExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                context.Response.ContentType = "application/problem+json";

                var title = "An error occured: " + ex.Message;
                var details = ex.ToString();

                var problem = new 
                {
                    Status = 200,
                    Title = title,
                    Detail = details
                };
                var stream = context.Response.Body;
                await JsonSerializer.SerializeAsync(stream, problem);
            }
        }
    }
}
