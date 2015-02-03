using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RedisCollections.Client
{
    public static class Serializer
    {
        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
            
        } 
        
        public static T Deserialize<T>(string serializedItem)
        {
            return JsonConvert.DeserializeObject<T>(serializedItem);
        }
    }
}
