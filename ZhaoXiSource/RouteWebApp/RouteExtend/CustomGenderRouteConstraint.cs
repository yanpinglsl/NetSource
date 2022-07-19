using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RouteWebApp.RouteExtend
{
    /// <summary>
    /// 自定义路由约束，只能是0/1
    /// 当然也可以有其他规则，比如必须是中文、英文等几个固定资源啥的
    /// </summary>
    public class CustomGenderRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            Console.WriteLine($"This is {nameof(CustomGenderRouteConstraint)}.Match...");
            if (values.TryGetValue(routeKey, out object value))
            {
                var parameterValueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (parameterValueString == null)
                {
                    return false;
                }
                else
                {
                    return parameterValueString.Equals("0") || parameterValueString.Equals("1");
                }
            }

            return false;
        }
    }
}
