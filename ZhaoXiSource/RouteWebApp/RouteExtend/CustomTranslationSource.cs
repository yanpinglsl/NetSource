using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteWebApp.RouteExtend
{
    /// <summary>
    /// 数据源
    /// </summary>
    public class CustomTranslationSource
    {
        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// http://localhost:5726/en/route/index
        /// http://localhost:5726/ch/route1/index1
        /// http://localhost:5726/hk/route2/index2
        /// 
        /// 映射规则，可以是任意数据源和任意配置
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> MappingRuleDictionary
            = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "en", new Dictionary<string, string>
                {
                    { "route", "Route" },
                    { "index", "Index" }
                }
            },
            {
                "ch", new Dictionary<string, string>
                {
                    { "route1", "Route" },
                    { "index1", "Index" }
                }
            },
            {
                "hk", new Dictionary<string, string>
                {
                    { "route2", "Route" },
                    { "index2", "Index" }
                }
            },
        };
        /// <summary>
        /// 根据区域，将控制器和Action，做个映射
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<string> Mapping(string lang, string value)
        {
            var area = lang.ToLowerInvariant();
            var mapValue = value.ToLowerInvariant();
            await Task.CompletedTask;
            if (MappingRuleDictionary.ContainsKey(area) && MappingRuleDictionary[area].ContainsKey(mapValue))
            {
                return MappingRuleDictionary[area][mapValue];
            }
            else
            {
                return null;
            }
        }
    }
}
