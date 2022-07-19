
using global::Microsoft.AspNetCore.Builder;
using global::Microsoft.AspNetCore.Http;
using global::Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SameSiteCookiesServiceCollectionExtensions
    {
        private const SameSiteMode Unspecified = (SameSiteMode)(-1);

        public static IServiceCollection AddNonBreakingSameSiteCookies(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = Unspecified;
                options.OnAppendCookie = cookieContext =>
                   CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                   CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            return services;
        }

        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                options.SameSite = ReplaceSameSiteNoneByUserAgent(userAgent);
            }
        }

        private static SameSiteMode ReplaceSameSiteNoneByUserAgent(string userAgent)
        {
            if (userAgent.Contains("CPU iPhone OS 12")
               || userAgent.Contains("iPad; CPU OS 12"))
            {
                return Unspecified;
            }
            if (userAgent.Contains("Safari")
               && userAgent.Contains("Macintosh; Intel Mac OS X 10_14")
               && userAgent.Contains("Version/"))
            {
                return Unspecified;
            }


            var ma = new Regex("Chrome/([0-9]+)").Match(userAgent);
            if (ma.Success && int.TryParse(ma.Groups[1].Value, out int chromeVer))
            {
                if (chromeVer >= 50 && chromeVer <= 69)
                {
                    return Unspecified;
                }
                else if (chromeVer >= 80)
                {
                    return SameSiteMode.Lax;
                }
            }

            return SameSiteMode.None;
        }
    }

}
