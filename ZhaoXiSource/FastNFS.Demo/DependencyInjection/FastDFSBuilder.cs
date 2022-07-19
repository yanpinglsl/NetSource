using FastNFS.Demo.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastNFS.Demo.DependencyInjection
{
    public class FastDFSBuilder : IFastDFSBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }

        public FastDFSBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;
            // 单例方式注入到IOC
            Services.TryAddSingleton<FastDFSProvider>();
        }
    }
}
