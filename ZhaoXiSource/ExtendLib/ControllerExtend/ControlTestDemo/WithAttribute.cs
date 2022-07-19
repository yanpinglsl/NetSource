using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ControllerExtend.ControlTestDemo
{
    /// <summary>
    /// 通过特性转成控制器
    /// </summary>
    [Controller]
    public class WithAttribute
    {
        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// 
        /// http://localhost:5726/WithAttribute/Get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            return "WithAttribute-Get 222";
        }
        /// <summary>
        /// 获取HttpContext，需要IOC注册
        /// </summary>
        private IHttpContextAccessor _IHttpContextAccessor;
        private HttpContext _HttpContext;
        public WithAttribute(IHttpContextAccessor httpContextAccessor)
        {
            this._IHttpContextAccessor = httpContextAccessor;
            this._HttpContext = this._IHttpContextAccessor.HttpContext;
        }

        /// <summary>
        /// http://localhost:5726/WithAttribute/Convention
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Convention()
        {
            return $"WithAttribute-Convention  ConventionValue={this._HttpContext.GetRouteValue("ConventionValue")}";
        }
    }

    /// <summary>
    /// http://localhost:5726/api/WithAttributeNot
    /// </summary>
    [NonController]
    [Route("api/[controller]")]
    public class WithAttributeNotController
    {
        [HttpGet]
        public string Get()
        {
            return "WithAttributeNotController-Get 333";
        }
    }
}
