using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisCollections.Client;

namespace RedisCollections.Extensions
{
    static class GenericExtensions
    {
        public static string SerializeToString<T>(this T t)
        {
            return Serializer.Serialize(t);
        }

        public static T ToOrDefaultValue<T>(this string toSerialize)
        {
            return Serializer.Deserialize<T>(toSerialize);
        }
    }
}
