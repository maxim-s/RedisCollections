using System;
using System.Collections;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Redis;

namespace RedisCollections
{
    public class RedisDictionary<TKey,TValue> : IDictionary<TKey, TValue>
    {
        private readonly RedisClient redisClient;

        public RedisDictionary(RedisClient redisClient)
        {
            this.redisClient = redisClient;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            if (!typeof(TKey).IsValueType() && key == null )
            {
                throw new ArgumentNullException("Key cannot be null");
            }

            if (redisClient.ContainsKey(key.SerializeToString()))
            {
                throw new ArgumentException("An item with the same key has already been added.");
            }
            redisClient.Set(key.SerializeToString(), value);
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get { return redisClient.Get<TValue>(key.SerializeToString()); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
    }
}
