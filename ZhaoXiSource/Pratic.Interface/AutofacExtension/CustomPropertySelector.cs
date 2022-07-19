using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Practice.Interface.AutofacExtension
{
    /// <summary>
    /// IPropertySelector:查看 属性上是否标记某一个特性
    /// </summary>
    public class CustomPropertySelector : IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            //需要一个判断的维度；  如果标记的有CustomPropertyAttribute特性 返回True：返回true；就构造实例
            return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(CustomPropertyAttribute));
        }
    }
}
