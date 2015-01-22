﻿using System;
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
        private readonly string instanceKey;
        private readonly string searchPattern;

        private string CreateKey(string key)
        {
            return string.Format("{0}{1}", instanceKey, key);
        }

        public RedisDictionary(RedisClient redisClient)
        {
            this.redisClient = redisClient;

            instanceKey = string.Format("{0}::", Guid.NewGuid());
            searchPattern = string.Format("{0}*", instanceKey);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        internal struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private IDictionary<TKey, TValue> dictionary;
            private readonly TKey[] keys;
            private int index;
            private KeyValuePair<TKey, TValue> current;

            internal Enumerator(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                this.keys = dictionary.Keys.ToArray();
                index = 0;
                this.current = new KeyValuePair<TKey, TValue>();
            }


            public void Dispose()
            {
                this.dictionary = null;
            }

            public bool MoveNext()
            {
                while ((uint)index < (uint)keys.Length)
                {
                    current = new KeyValuePair<TKey, TValue>(keys[index], dictionary[keys[index]]);
                    index++;
                    return true;
                }

                index = keys.Length + 1;
                current = new KeyValuePair<TKey, TValue>();
                return false;
            }

            public void Reset()
            {
                index = 0;
                current = new KeyValuePair<TKey, TValue>();
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return GetCurrent(); }
            }

            private KeyValuePair<TKey, TValue> GetCurrent()
            {
                CheckState();
                return current;
            }

            object IEnumerator.Current
            {
                get { return GetCurrent(); }
            }

            private void CheckState()
            {
                if (index == 0 || (index == keys.Length + 1))
                {
                    throw new InvalidOperationException("Can't perfom enumerate operation");
                }
            }
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
                    pair.Key.TrimPrefixes(instanceKey).ToOrDefaultValue<TKey>(), pair.Value);

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
            set { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return GetRedisKeys()
                           .Select(a => a.TrimPrefixes(instanceKey).ToOrDefaultValue<TKey>())
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
