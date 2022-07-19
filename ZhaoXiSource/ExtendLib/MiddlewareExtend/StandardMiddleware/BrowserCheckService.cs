using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ExtendLib.MiddlewareExtend.StandardMiddleware
{
    public class BrowserCheckService : IBrowserCheck
    {
        //private readonly BrowserFilterOptions _BrowserFilterOptions;
        private readonly ILogger _logger;

        public BrowserCheckService(ILogger<BrowserCheckService> logger)
        {
            this._logger = logger;
            //this._BrowserFilterOptions = options.CurrentValue;
        }


        public Tuple<bool, string> CheckBrowser(HttpContext httpContext, BrowserFilterOptions options)
        {
            Console.WriteLine($"EnableChorme={options.EnableChorme}");
            Console.WriteLine($"EnableEdge={options.EnableEdge}");
            Console.WriteLine($"EnableFirefox={options.EnableFirefox}");
            Console.WriteLine($"EnableIE={options.EnableIE}");


            if (httpContext.Request.Headers["User-Agent"].Contains("Edg/") && !options.EnableEdge)
            {
                Console.WriteLine($"{nameof(BrowserFilterMiddleware)} Refuse Edge,Choose other one<br/>");
                return Tuple.Create(false, $"{nameof(BrowserFilterMiddleware)} Refuse Edge,Choose other one<br/>");
            }
            else
            {
                return Tuple.Create(true, $"{nameof(BrowserFilterMiddleware)} ok");
            }
        }
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.64
    }
}
