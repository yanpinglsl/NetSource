using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteWebApp.RouteExtend
{
    /// <summary>
    /// 扩展动态路由，数据源可以自定义
    /// </summary>
    public class TranslationTransformer : DynamicRouteValueTransformer
    {
        private readonly CustomTranslationSource _CustomTranslationSource;
        public TranslationTransformer(CustomTranslationSource translationSource)
        {
            this._CustomTranslationSource = translationSource;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext
        , RouteValueDictionary values)
        {
            if (!values.ContainsKey("language")
                || !values.ContainsKey("controller")
                || !values.ContainsKey("action")) return values;

            var language = (string)values["language"];
            var controller = await this._CustomTranslationSource.Mapping(language,
                (string)values["controller"]);

            if (controller == null) return values;
            values["controller"] = controller;

            var action = await this._CustomTranslationSource.Mapping(language,
                (string)values["action"]);

            if (action == null) return values;
            values["action"] = action;

            return values;
        }
    }
}
