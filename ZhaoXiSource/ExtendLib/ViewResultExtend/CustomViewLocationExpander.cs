using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ViewResultExtend
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        /// <summary>
        /// 也一定执行---提供模板
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewLocations"></param>
        /// <returns></returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            Console.WriteLine($"This is CustomViewLocationExpander ExpandViewLocations  {context.ControllerName}+{context.PageName}");

            var template = context.Values["Style"] ?? "";//"Default";//"";//

            string[] locations = {
                "/Views/" + template + "/{1}/{0}.cshtml",
                "/Views/" + template + "/{0}.cshtml",
                "/Views/" + template + "/Shared/{0}.cshtml"
            };
            Console.WriteLine(string.Join(",", locations));
            return locations.Union(viewLocations);
        }


        /// <summary>
        /// 先执行一遍，填充值，就是为了扩展的
        /// </summary>
        /// <param name="context"></param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //有ViewLocationExpanderContext--有了HttpContext，为所欲为
            Console.WriteLine($"This is CustomViewLocationExpander PopulateValues  {context.ControllerName}+{context.PageName}");
            context.Values["Style"] = context.ActionContext.HttpContext.Request.Query["Style"];

            #region 移动端
            //var userAgent = context.ActionContext.HttpContext.Request.Headers["User-Agent"].ToString();
            //if (this.IsMobile(userAgent))
            //{
            //    context.Values["Style"] = "m";
            //}
            //else
            //{
            //    context.Values["Style"] = "";
            //}
            #endregion
        }


        /// <summary>
        /// 判断是否是移动端
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        protected bool IsMobile(string userAgent)
        {
            userAgent = userAgent.ToLower();
            if (userAgent == "" ||
                userAgent.IndexOf("mobile") > -1 ||
                userAgent.IndexOf("mobi") > -1 ||
                userAgent.IndexOf("nokia") > -1 ||
                userAgent.IndexOf("samsung") > -1 ||
                userAgent.IndexOf("sonyericsson") > -1 ||
                userAgent.IndexOf("mot") > -1 ||
                userAgent.IndexOf("blackberry") > -1 ||
                userAgent.IndexOf("lg") > -1 ||
                userAgent.IndexOf("htc") > -1 ||
                userAgent.IndexOf("j2me") > -1 ||
                userAgent.IndexOf("ucweb") > -1 ||
                userAgent.IndexOf("opera mini") > -1 ||
                userAgent.IndexOf("android") > -1 ||
                userAgent.IndexOf("transcoder") > -1)
            {
                return true;
            }

            return false;
        }
    }
}
