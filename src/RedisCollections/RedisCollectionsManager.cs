using System.Collections.Generic;
using RedisCollections.Client;

namespace RedisCollections
{
    public class RedisCollectionsManager
    {
        private readonly IRedisClient redisClient;

        public RedisCollectionsManager(IRedisClient redisClient)
        {
            this.redisClient = redisClient;
        }

        public IDictionary<TKey, TValue> GetDictionary<TKey, TValue>(string nameSpace = null)
        {
            return new RedisDictionary<TKey, TValue>(redisClient, nameSpace);
        }

        public IList<T> GetList<T>(string listName)
        {
            return new RedisList<T>(redisClient, listName);
        }

        public ISet<T> GetSet<T>()
        {
            return new RedisSet<T>(redisClient);
        }
    }
}
