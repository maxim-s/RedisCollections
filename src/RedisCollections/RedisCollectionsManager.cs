using System.Collections.Generic;
using ServiceStack.Redis;

namespace RedisCollections
{
    public class RedisCollectionsManager
    {
        private readonly RedisClient redisClient;

        public RedisCollectionsManager(RedisClient redisClient)
        {
            this.redisClient = redisClient;
        }

        public IDictionary<TKey, TValue> GetDictionary<TKey, TValue>(string nameSpace = null)
        {
            return new RedisDictionary<TKey, TValue>(redisClient, nameSpace);
        }
    }
}
