using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.LogExtend
{
    /// <summary>
    /// 数据源：负责创建ConfigurationProvider--数据提供程序
    /// </summary>
    public class CustomConfigurationSource : IConfigurationSource
    {
        private readonly Action<CustomConfigurationOption> _optionsAction;

        public CustomConfigurationSource(Action<CustomConfigurationOption> optionsAction)
        {
            _optionsAction = optionsAction;
        }
        /// <summary>
        /// 创建provider
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            CustomConfigurationOption customConfigurationOption=new CustomConfigurationOption();
            this._optionsAction.Invoke(customConfigurationOption);

            return new CustomConfigurationProvider(customConfigurationOption);
        }
    }
}
