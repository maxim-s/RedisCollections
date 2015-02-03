using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RedisCollections.Client;

namespace RedisCollections
{
    public class RedisList<T> : IList<T>
    {
        private readonly IRedisClient redisClient;
        private readonly string list;

        public RedisList(IRedisClient redisClient)
        {
            this.redisClient = redisClient;
        }

        public RedisList(IRedisClient redisClient, string list)
        {
            this.redisClient = redisClient;
            this.list = list;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Range(0, Count).Select(c => this[c]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            redisClient.RPush(list, item);
        }

        public void Clear()
        {
            redisClient.Del(list);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
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

            for (var i = 0; i < array.Length - arrayIndex; i++)
            {
                array[i + arrayIndex] = this[i];
            }
        }

        public bool Remove(T item)
        {
            return redisClient.LRem(list, item);
        }

        public int Count 
        { 
            get
            {
                return redisClient.LLen(list);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < Count; i++)
            {
                if (comparer.Equals(item, this[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            var value = this[index];
            Remove(value);
        }

        public T this[int index]
        {
            get { return redisClient.LIndex<T>(list, index); }
            set { redisClient.LSet(list, index, value); }
        }
    }
}
