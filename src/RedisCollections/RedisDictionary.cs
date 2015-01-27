using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Redis;

namespace RedisCollections
{
    public class RedisDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly RedisClient redisClient;
        private readonly string nameSpace;
        private const string GlobalNameSpace = "DICTIONARY-GLOBAL";

        public string NameSpace
        {
            get { return nameSpace; }
        }

        private readonly string searchPattern;

        private string CreateKey(string key)
        {
            return string.Format("{0}{1}", nameSpace, key);
        }

        internal RedisDictionary(RedisClient redisClient)
        {
            this.redisClient = redisClient;

            nameSpace = string.Format("{0}::", GlobalNameSpace);
            searchPattern = string.Format("{0}*", nameSpace);
        }

        internal RedisDictionary(RedisClient redisClient, string nameSpace)
        {
            this.redisClient = redisClient;
            this.nameSpace = string.Format("{0}::", string.IsNullOrWhiteSpace(nameSpace)?GlobalNameSpace:nameSpace);
            searchPattern = string.Format("{0}*", this.nameSpace);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Keys.Select(key => new KeyValuePair<TKey, TValue>(key,this[key])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            var keys = SearchKeys();
            if (keys.Count > 0)
            {
                redisClient.RemoveAll(keys);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue val;
            if (TryGetValue(item.Key, out val))
            {
                return EqualityComparer<TValue>.Default.Equals(val, item.Value);
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array can't be null");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentException("incorrect index value");
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Array is too small");
            }

            redisClient.GetAll<TValue>(GetRedisKeys()).Each((i, pair) =>
            {
                array[i] = new KeyValuePair<TKey, TValue>(
                    pair.Key.TrimPrefixes(nameSpace).ToOrDefaultValue<TKey>(), pair.Value);

            });
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            CheckForNull(item.Key);

            return Contains(item) && RemoveCore(item.Key);
        }

        public int Count
        {
            get { return SearchKeys().Count; }
        }

        private List<string> SearchKeys()
        {
            return redisClient.SearchKeys(searchPattern);
        }

        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(TKey key)
        {
            return redisClient.ContainsKey(CreateKey(key.SerializeToString()));
        }

        public void Add(TKey key, TValue value)
        {
            CheckForNull(key);

            if (ContainsKey(key))
            {
                throw new ArgumentException("An item with the same key has already been added.");
            }
            redisClient.Set(CreateKey(key.SerializeToString()), value);
        }

        public bool Remove(TKey key)
        {
            CheckForNull(key);

            return RemoveCore(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }
            value = this[key];
            return true;
        }

        public TValue this[TKey key]
        {
            get { return redisClient.Get<TValue>(CreateKey(key.SerializeToString())); }
            set { redisClient.Set(CreateKey(key.SerializeToString()), value); }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return GetRedisKeys()
                           .Select(a => a.TrimPrefixes(nameSpace).ToOrDefaultValue<TKey>())
                           .ToList();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var keys = GetRedisKeys();
                return redisClient.GetValues<TValue>(keys);
            }
        }

        private List<string> GetRedisKeys()
        {
            return redisClient.SearchKeys(searchPattern);
        }

        private bool RemoveCore(TKey key)
        {
            return redisClient.Del(CreateKey(key.SerializeToString())) > 0;
        }

        private static void CheckForNull(TKey key)
        {
            if (!typeof(TKey).IsValueType() && key == null)
            {
                throw new ArgumentNullException("Key cannot be null");
            }
        }
    }
}
