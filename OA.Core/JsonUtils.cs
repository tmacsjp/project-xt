using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace OA.Core
{
    public class JsonUtils
    {
        public static T ConvertTo<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        public static T ToModel<T>(string json)
        {
            return ConvertTo<T>(json);
        }

        public static T ToModel<T>(object model)
        {
            return ConvertTo<T>(ToJson(model));
        }
        public static object ToModel(string json, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
        }

        public static JToken ConvertToObject(string json)
        {
            return Newtonsoft.Json.Linq.JToken.Parse(json);
        }

        public static string ToJson(object obj)
        {
            return ToJson(obj, false);
        }
        public static string ToJsonBeauty(object obj)
        {
            return ToJson(obj, true);
        }
        public static string ToJson(object obj, bool indented)
        {
            var setting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new JsonPropertyContractResolver(),
                NullValueHandling = NullValueHandling.Include,
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None, setting);
        }
        public static string ToJson(object obj, JsonSerializerSettings setting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, setting);
        }

        public static string ToJson(object obj, bool indented, IEnumerable<string> ignores)
        {
            var setting = new Newtonsoft.Json.JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new JsonPropertyContractResolver(null, ignores),
                NullValueHandling = NullValueHandling.Include
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None, setting);
        }

        public class JsonPropertyContractResolver : CamelCasePropertyNamesContractResolver
        {
            IEnumerable<string> lstInclude;
            IEnumerable<string> lstIgnore;
            public JsonPropertyContractResolver() { }
            public JsonPropertyContractResolver(IEnumerable<string> includeProperties, IEnumerable<string> ignoreProperties)
            {
                lstInclude = includeProperties;
                lstIgnore = ignoreProperties;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var list = base.CreateProperties(type, memberSerialization);
                if (lstInclude != null)
                {
                    return list.Where(p => lstInclude.Contains(p.PropertyName)).ToList();//需要输出的属性  } }
                }
                if (lstIgnore != null)
                {
                    return list.Where(p => !lstIgnore.Contains(p.PropertyName)).ToList();//需要输出的属性  } }
                }
                return list;
            }
        }

        /// <summary>
        /// 递归查找子节点的值
        /// </summary>
        /// <param name="containerToken"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<JToken> FindTokens(JToken containerToken, string name)
        {
            List<JToken> matches = new List<JToken>();
            try
            {
                FindTokens(containerToken, name, matches);
            }
            catch (Exception)
            {
                // ignored
            }
            return matches;
        }

        private static void FindTokens(JToken containerToken, string name, List<JToken> matches)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    if (child.Name == name)
                    {
                        matches.Add(child.Value);
                    }
                    FindTokens(child.Value, name, matches);
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, name, matches);
                }
            }
        }


    }
}
