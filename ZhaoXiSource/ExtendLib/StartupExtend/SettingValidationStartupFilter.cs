using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtendLib.StartupExtend
{
    public interface IValidatable
    {
        void Validate();
    }
    /// <summary>
    /// SettingValidationStartupFilter并没有修改任何中间件管道, Configure方法中直接返回了next对象。
    /// 但是如果某个强类型配置类的验证失败，在程序启动时，就会抛出异常，从而阻止了程序
    /// </summary>
    public class SettingValidationStartupFilter : IStartupFilter
    {
        readonly IEnumerable<IValidatable> _validatableObjects;
        public SettingValidationStartupFilter(IEnumerable<IValidatable> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var validatableObject in _validatableObjects)
            {
                validatableObject.Validate();
            }

            return next;
        }
    }
}
