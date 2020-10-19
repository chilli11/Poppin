using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Poppin.Models.Yelp;

namespace Poppin
{
				public static class GeneralExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

        public static IDictionary<string, string> AsStringDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var output = new Dictionary<string, string>();
            var sType = source.GetType();
            var props = sType.GetProperties(bindingAttr);
            foreach (var propInfo in props)
            {
                var val = propInfo.GetValue(source);
                if (val != null)
                {
                    output.Add(propInfo.Name, val.ToString());
                }
            }

            return output;
        }

        private static string AddIfNotNull(this string str, string key, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                str += key + "=" + val + ";";
            }
            return str;
        }
    }
}
