using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RedisCollections.Client
{
    public interface IRedisClient
    {
        void RemoveAll(List<string> keys);
        IDictionary<string, T> GetAll<T>(List<string> keys);
        List<string> SearchKeys(string searchPattern);
        bool ContainsKey(string key);
        void Set<T>(string key, T value);
        T Get<T>(string key);
        ICollection<T> GetValues<T>(List<string> keys);
        bool Del(string key);
        void FlushAll();
        void Dispose();

        void Add<T>(string list, params T[] values);
        bool Remove<T>(string list, T obj);
        int Count(string list);
        T Index<T>(string list, int index);
        void Set<T>(string list, int index, T obj);
    }
}
