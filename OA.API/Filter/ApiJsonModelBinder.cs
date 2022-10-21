using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OA.API.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.API.Filter
{
    public class ApiJsonModelBinder : IModelBinder
    {
        public static readonly ApiJsonModelBinder Instance = new ApiJsonModelBinder();
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var provider = bindingContext.HttpContext.RequestServices;
            var jsonReader = provider.GetRequiredService<IReqeustJsonBodyReader>();

            var jbody = await jsonReader.Read(provider, bindingContext.HttpContext);
            if (jbody == null)
                return;
            var modelValue = Bind(jbody, bindingContext.ModelType, bindingContext.OriginalModelName);
            if (modelValue != null)
            {
                bindingContext.Result = ModelBindingResult.Success(modelValue);
            }
            return;
        }


        static Type[] SimpleTypes = new Type[] {
                    typeof(string),typeof(Int32), typeof(Boolean), typeof(Byte), typeof(Char),
                    typeof(DateTime), typeof(Decimal), typeof(Double),
                    typeof(SByte), typeof(DateTimeOffset), typeof(Guid), typeof(Int16),
                    typeof(Int64), typeof(Single), typeof(TimeSpan), typeof(UInt16), typeof(UInt32), typeof(UInt64),
                    typeof(Uri), typeof(Version) };
        private static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.Name == "Nullable`1")
            {
                type = type.GenericTypeArguments[0];
            }
            if (type.IsEnum || SimpleTypes.Contains(type))
                return true;
            return false;

        }

        public static object Bind(JToken jsonPara, Type type, string name)
        {
            bool simpleType = IsSimpleType(type);
            object value = null;
            try
            {
                if (simpleType)
                {
                    value = GetValueFromJToken(jsonPara, name).ToObject(type);
                }
                else
                {
                    value = jsonPara.ToObject(type);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return value;
        }

        private static JToken GetValueFromJToken(JToken jToken, string key)
        {
            if (jToken is JObject jObject)
            {
                if (jObject.TryGetValue(key, StringComparison.CurrentCultureIgnoreCase, out var jValue))
                {
                    return jValue;
                }
            }
            return null;
        }
    }
}
