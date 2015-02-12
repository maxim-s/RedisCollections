using System.Collections.Generic;
using System.Linq;
using CSRedis;

namespace RedisCollections.Client
{
    public class Redis : IRedisClient
    {
        private readonly RedisClient redisClient;

        public Redis(RedisClient redisClient)
        {
            this.redisClient = redisClient;
        }

        public void RemoveAll(List<string> keys)
        {
            redisClient.Del(keys.ToArray());
        }

        public IDictionary<string, T> GetAll<T>(List<string> keys)
        {
            var keysArr = keys.ToArray();
            var stringValues = redisClient.MGet(keysArr);
            var result = new Dictionary<string, T>();
            if (stringValues != null)
            {
                for (int i = 0; i < stringValues.Length; i++)
                {
                    result.Add(keysArr[i], Serializer.Deserialize<T>(stringValues[i]));
                }
            }
            return result;
        }

        public List<string> SearchKeys(string searchPattern)
        {
            return new List<string>(redisClient.Keys(searchPattern));
        }

        public bool ContainsKey(string key)
        {
            return redisClient.Exists(key);
        }

        public void Set<T>(string key, T value)
        {
            redisClient.Set(key, Serializer.Serialize(value));
        }

        public T Get<T>(string key)
        {
            return Serializer.Deserialize<T>(redisClient.Get(key));
        }

        public ICollection<T> GetValues<T>(List<string> keys)
        {
            var stringValues = redisClient.MGet(keys.ToArray());
            return stringValues.Select(Serializer.Deserialize<T>).ToList();
        }

        public bool Del(string key)
        {
            return redisClient.Del(new[] { key }) > 0;
        }

        public void FlushAll()
        {
            redisClient.FlushAll();
        }

        public void Dispose()
        {
            redisClient.Dispose();
        }

        public bool AddToSet<T>(string name,T item)
        {
            return redisClient.SAdd(name, item) == 1;
        }

        public bool ContainsInSet<T>(string name,T item)
        {
            return redisClient.SIsMember(name, item);
        }
    }
}