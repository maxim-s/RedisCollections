using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCollections.Client
{
    public interface IRedisClient
    {
        void RemoveAll(List<string> keys);
        IDictionary<string,T> GetAll<T>(List<string> getRedisKeys);
        List<string> SearchKeys(string searchPattern);
        bool ContainsKey(string createKey);
        void Set<T>(string createKey, T value);
        T Get<T>(string createKey);
        ICollection<T> GetValues<T>(List<string> keys);
        bool Del(string createKey);
        void FlushAll();
        void Dispose();
    }
}
