using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.LogExtend
{
    /// <summary>
    /// 核心数据提供程序
    /// </summary>
    public class CustomConfigurationProvider : ConfigurationProvider
    {
        private CustomConfigurationOption _CustomConfigurationOption=null;
        public CustomConfigurationProvider(CustomConfigurationOption customConfigurationOption)
        {
            this._CustomConfigurationOption= customConfigurationOption;
        }

        /// <summary>
        /// 启动加载数据即可
        /// </summary>
        public override void Load()
        {
            Console.WriteLine($"CustomConfigurationProvider load data");
            //当然也可以从数据库读取
            //var result = this._CustomConfigurationOption.DataInitFunc.Invoke();
            base.Data.Add("TodayCustom", "0625-Custom");
            base.Data.Add("RabbitMQOptions-Custom:HostName", "192.168.3.254-Custom");
            base.Data.Add("RabbitMQOptions-Custom:UserName", "guest-Custom");
            base.Data.Add("RabbitMQOptions-Custom:Password", "guest-Custom");
            base.Data.Add("RabbitMQOptions-Custom:LogTag", this._CustomConfigurationOption.LogTag);
        }

        //其他的用的默认

        public override void Set(string key, string value)
        {
            base.Data.Add(key, value);
            //this._CustomConfigurationOption.DataChangeAction(key, value);
        }
    }
}
