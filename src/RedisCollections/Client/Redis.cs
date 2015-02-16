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

        public void Add<T>(string list, params T[] values)
        {
            redisClient.RPush(list, values.Select(Serializer.Serialize).ToArray());
        }

        public bool Remove<T>(string list, T obj)
        {
            return redisClient.LRem(list, -1, Serializer.Serialize(obj)) > 0;
        }

        public int Count(string list)
        {
            return (int)redisClient.LLen(list);
        }

        public T Index<T>(string list, int index)
        {
            return Serializer.Deserialize<T>(redisClient.LIndex(list, index));
        }

        public void Set<T>(string list, int index, T obj)
        {
            redisClient.LSet(list, index, Serializer.Serialize(obj));
        }

		public IEnumerable<T> Range<T>(string list, int start, int stop)
	    {
			return redisClient.LRange(list, start, stop).Select(Serializer.Deserialize<T>);
	    }

	    public void Multi()
	    {
		    redisClient.Multi();
	    }

		public void Exec()
		{
			redisClient.Exec();
		}

	    public void Trim(string list, int start, int stop)
	    {
		    redisClient.LTrim(list, start, stop);
	    }

	    public void Push<T>(string list, IEnumerable<T> collection)
	    {
			redisClient.RPush(list, collection.Select(a => (object) Serializer.Serialize(a)).ToArray());
	    }
    }
}