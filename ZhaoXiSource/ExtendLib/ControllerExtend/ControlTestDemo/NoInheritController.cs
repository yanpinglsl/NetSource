using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ControllerExtend.ControlTestDemo
{
    /// <summary>
    /// http://localhost:5726/api/NoInherit
    /// </summary>
    [Route("api/[controller]")]
    public class NoInheritController
    {
        [HttpGet]
        public string Get()
        {
            return "NoInheritController 111";
        }
    }

    /// <summary>
    /// http://localhost:5726/NoInheritWithView/Index
    /// </summary>
    public class NoInheritWithViewController
    {
        /// <summary>
        /// 没有继承Controller，viewdata不能用，所以注入个IModelMetadataProvider
        /// 要获取HttpContext---IHttpContextAccessor
        /// </summary>
        private IModelMetadataProvider _IModelMetadataProvider = null;
        public NoInheritWithViewController(IModelMetadataProvider modelMetadataProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._IModelMetadataProvider = modelMetadataProvider;
            //httpContextAccessor.HttpContext
        }


        [HttpGet]
        public IActionResult Index()
        {
            ViewDataDictionary viewData = new ViewDataDictionary(this._IModelMetadataProvider, new ModelStateDictionary());
            viewData["Info"] = $"{nameof(NoInheritWithViewController)}.Info";

            return new ViewResult()
            {
                ViewName = "~/Views/Part/Index.cshtml",
                ViewData = viewData
            };

        }
    }

}