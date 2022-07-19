using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastNFS.Demo.DependencyInjection
{
    public interface IFastDFSBuilder
    {
        IServiceCollection Services { get; }

        IConfiguration Configuration { get; }
    }
}
