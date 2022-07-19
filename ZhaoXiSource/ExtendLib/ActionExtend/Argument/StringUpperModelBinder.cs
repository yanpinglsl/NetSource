using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace ExtendLib.ActionExtend.Argument
{
    /// <summary>
    /// 具体如何绑定
    /// </summary>
    public class StringUpperModelBinder : IModelBinder
    {
        private readonly TypeConverter _typeConverter;

        /// <summary>
        /// Initializes a new instance of <see cref="SimpleTypeModelBinder"/>.
        /// </summary>
        /// <param name="type">The type to create binder for.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public StringUpperModelBinder(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            _typeConverter = TypeDescriptor.GetConverter(type);
        }

        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            //读取数据源
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            try
            {
                var value = valueProviderResult.FirstValue.ToUpper();//把值大写了
                Console.WriteLine($"***********modelName:{value}*************");

                object? model = value;
                //if (bindingContext.ModelType == typeof(string))
                //{
                //    // Already have a string. No further conversion required but handle ConvertEmptyStringToNull.
                //    if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && string.IsNullOrWhiteSpace(value))
                //    {
                //        model = null;
                //    }
                //    else
                //    {
                //        model = value;
                //    }
                //}
                //else if (string.IsNullOrWhiteSpace(value))
                //{
                //    // Other than the StringConverter, converters Trim() the value then throw if the result is empty.
                //    model = null;
                //}
                //else
                //{
                //    model = _typeConverter.ConvertFrom(
                //        context: null,
                //        culture: valueProviderResult.Culture,
                //        value: value);
                //}

                CheckModel(bindingContext, valueProviderResult, model);

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                var isFormatException = exception is FormatException;
                if (!isFormatException && exception.InnerException != null)
                {
                    // TypeConverter throws System.Exception wrapping the FormatException,
                    // so we capture the inner exception.
                    exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
                }

                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    exception,
                    bindingContext.ModelMetadata);

                // Were able to find a converter for the type but conversion failed.
                return Task.CompletedTask;
            }
        }

        /// <inheritdoc/>
        protected virtual void CheckModel(
            ModelBindingContext bindingContext,
            ValueProviderResult valueProviderResult,
            object? model)
        {
            // When converting newModel a null value may indicate a failed conversion for an otherwise required
            // model (can't set a ValueType to null). This detects if a null model value is acceptable given the
            // current bindingContext. If not, an error is logged.
            if (model == null && !bindingContext.ModelMetadata.IsReferenceOrNullableType)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    bindingContext.ModelMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor(
                        valueProviderResult.ToString()));
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(model);
            }
        }
    }
}
