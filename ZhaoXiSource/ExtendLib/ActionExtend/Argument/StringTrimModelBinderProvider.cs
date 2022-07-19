using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ActionExtend.Argument
{
    /// <summary>
    /// 参数Binder的Provider
    /// </summary>
    public class StringTrimModelBinderProvider : IModelBinderProvider
    {
        private readonly IList<IInputFormatter> _formatters;

        public StringTrimModelBinderProvider(IList<IInputFormatter> formatters)
        {
            _formatters = formatters;
        }

        /// <summary>
        /// 只处理最基础字符串参数的
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(string))
            {
                //简单类型
                return new StringUpperModelBinder(context.Metadata.ModelType);
            }
            //else if (context.BindingInfo.BindingSource != null &&
            //    context.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body))
            //{
            //    //通过[FromBody]绑定的
            //    return new BodyStringTrimModelBinder(_formatters, context.Services.GetRequiredService<IHttpRequestStreamReaderFactory>());
            //}
            return null;
        }
    }
}
