using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.MiddlewareExtend.StandardMiddleware
{
    public interface IBrowserCheck
    {
        Tuple<bool, string> CheckBrowser(HttpContext httpContext, BrowserFilterOptions options);
    }
}
