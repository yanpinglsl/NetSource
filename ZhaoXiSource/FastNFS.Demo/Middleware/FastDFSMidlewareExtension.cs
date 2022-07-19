using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastNFS.Demo.Middleware
{
    public static class FastDFSMidlewareExtension
    {
        public static IApplicationBuilder UseFastDFS(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FastDFSMiddleware>();
        }
    }
}
