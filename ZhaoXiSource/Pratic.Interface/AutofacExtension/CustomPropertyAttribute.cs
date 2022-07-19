using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Practice.Interface.AutofacExtension
{
    [AttributeUsage(AttributeTargets.Property)]//为了支持属性注入，只能标记在属性上
    public class CustomPropertyAttribute:Attribute
    {
    }
}
