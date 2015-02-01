using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCollections.Extensions
{
    static class GenericExtensions
    {
        public static string SerializeToString<T>(this T t)
        {
            return string.Empty;
        }

        public static T ToOrDefaultValue<T>(this string toSerialize)
        {
            return default(T);
        }
    }
}
