using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ServiceStack;
using ServiceStack.Redis;

namespace RedisCollections
{
    public class RedisDictionary<TKey,TValue> : IDictionary<TKey, TValue>
    {
        private readonly RedisClient redisClient;
        private readonly string instanceKey = Guid.NewGuid().ToString();
        private readonly string searchPattern;

        private string CreateKey(string key)
        {
            return string.Format("{0}::{1}", instanceKey, key);
        }

        public RedisDictionary(RedisClient redisClient)
        {
            this.redisClient = redisClient;
            searchPattern = string.Format("{0}::*", instanceKey);
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

        public int Count
        {
            get { return redisClient.SearchKeys(searchPattern).Count; }
        }

        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(TKey key)
        {
            return redisClient.ContainsKey(CreateKey(key.SerializeToString()));
        }

        public void Add(TKey key, TValue value)
        {
            if (!typeof(TKey).IsValueType() && key == null )
            {
                throw new ArgumentNullException("Key cannot be null");
            }

            if (redisClient.ContainsKey(CreateKey(key.SerializeToString())))
            {
                throw new ArgumentException("An item with the same key has already been added.");
            }
            redisClient.Set(CreateKey(key.SerializeToString()), value);
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
            get { return redisClient.Get<TValue>(CreateKey(key.SerializeToString())); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }
    }
}
