using System;
using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack.Redis;

namespace RedisCollections.Test
{
    [TestFixture]
    public class RedisDictionaryTest
    {
        private static RedisClient  redisClient;

        [TestFixtureSetUp]
        public void Init()
        {
            redisClient = new RedisClient();

        }

        [TestFixtureTearDown]
        public static void Destroy()
        {
            redisClient.Dispose();
        }

        [SetUp]
        public void Before()
        {
            redisClient.FlushAll();
        }


        [Test]
        public void Constructor_NonEmptyNameSpace_ShouldBeApplied()
        {
            var dictionaryWithNs = new RedisDictionary<string, string>(redisClient, "ns123") { { "key1", "val1" } };
            var dictionaryAutoNs = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" } };
            Assert.IsTrue(dictionaryWithNs.NameSpace.Contains("ns123"));
            Assert.IsFalse(dictionaryAutoNs.NameSpace.Contains("ns123"));
            Assert.AreEqual(1, dictionaryAutoNs.Count);
            Assert.AreEqual(1, dictionaryWithNs.Count);
        }

        [Test]
        public void Add_ItemAdded()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" } };
            Assert.AreEqual(dictionary["key1"], "val1");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_AddExistingKey_ArgumentExceptionThrown()
        {

            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" } };
            dictionary.Add("key1", "val2");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_KeyIsNull_ArgumentExceptionThrown()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { null, "val1" } };
        } 
        
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_NegativeAsIndexParam_ArgumentExceptionThrown()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, {"key2","val2"} };
            dictionary.CopyTo(new KeyValuePair<string, string>[2], -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_IndexGreaterThanArrayLength_ArgumentExceptionThrown()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val2" } };
            dictionary.CopyTo(new KeyValuePair<string, string>[2], 4);
        }

        [Test]
        public void CopyTo_SimpleValues_Copied()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val2" } };
            var keyValuePairs = new KeyValuePair<string, string>[2];
            dictionary.CopyTo(keyValuePairs, 0);
            Assert.AreEqual("key1",keyValuePairs[0].Key);
            Assert.AreEqual("val1",keyValuePairs[0].Value);
            Assert.AreEqual("key2",keyValuePairs[1].Key);
            Assert.AreEqual("val2",keyValuePairs[1].Value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_ArrayLengthLessThanAvailableItemsCount_ArgumentExceptionThrown()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val2" } };
            dictionary.CopyTo(new KeyValuePair<string, string>[1], 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyTo_NullAsArrayParam_ArgumentNullExceptionThrown()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val2" } };
            dictionary.CopyTo(null, 0);
        }

        [Test]
        public void DifferentDictionariesCanHasItemsWithTheSameKey()
        {
            var dictionary1 = new RedisDictionary<string, string>(redisClient) { { "key", "val1" } };
            var dictionary2 = new RedisDictionary<string, string>(redisClient) { { "key", "val2" } };
            Assert.AreNotEqual(dictionary1["key"], dictionary2["key"]);
        }

        [Test]
        public void Count_NumberOfItems()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            new RedisDictionary<string, string>(redisClient) { { "key", "val2" } };
            Assert.AreEqual(2, dictionary.Count);
        }

        [Test]
        public void ContainsKey_NotExistingKey_False()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            Assert.IsFalse(dictionary.ContainsKey("NotExistingKey"));
        }

        [Test]
        public void ContainsKey_ExistingKey_True()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            Assert.IsTrue(dictionary.ContainsKey("key1"));
            Assert.IsTrue(dictionary.ContainsKey("key2"));
        }

        [Test]
        public void ShouldReturnFalseIfCantGetValue()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            string res;
            Assert.IsFalse(dictionary.TryGetValue("NotExistingKey", out res));
            Assert.IsTrue(string.IsNullOrEmpty(res));
        }

        [Test]
        public void TryGetValue_ExistingKey_True()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            string res;
            Assert.IsTrue(dictionary.TryGetValue("key1", out res));
            Assert.AreEqual("val1", res);
        }

        [Test]
        public void Clear_AllItemsAreRemoved()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            dictionary.Clear();
            Assert.False(dictionary.ContainsKey("key1"));
            Assert.False(dictionary.ContainsKey("key2"));
            Assert.AreEqual(0, dictionary.Count);
        }

        [Test]
        public void Add_NewKeyValuePair_Added()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);
            Assert.IsTrue(dictionary.ContainsKey("key1"));
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual("val1", dictionary["key1"]);
        }

        [Test]
        public void Contains_NotExistingKeyValuePair_False()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            Assert.IsFalse(dictionary.Contains(kvp));
        }

        [Test]
        public void Contains_ExistingKeyValuePair_True()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);
            Assert.IsTrue(dictionary.Contains(kvp));
            Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, string>("key1", "val2")));
        }

        [Test]
        public void Remove_KeyExists_True()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);

            Assert.IsTrue(dictionary.Remove("key1"));
        }

        [Test]
        public void Remove_KeyExists_DeleteKey()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);

            dictionary.Remove("key1");

            Assert.IsFalse(dictionary.ContainsKey("key1"));
        }

        [Test]
        public void Remove_KeyAbsent_DeleteKey()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);

            dictionary.Remove("key2");

            Assert.IsFalse(dictionary.ContainsKey("key2"));
        }

        [Test]
        public void Remove_KeyNotExisits_False()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);

            Assert.IsFalse(dictionary.Remove("1"));
        }

        [Test]
        public void Keys_AddedKeys()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var kvp1 = new KeyValuePair<string, string>("key2", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);
            dictionary.Add(kvp);
            dictionary.Add(kvp1);

            CollectionAssert.AreEquivalent(new[] { "key1", "key2" }, dictionary.Keys);
        }

        [Test]
        public void Keys_ValueKeyType_AddedKeys()
        {
            var kvp = new KeyValuePair<int, string>(1, "val1");
            var kvp1 = new KeyValuePair<int, string>(2, "val1");
            var dictionary = new RedisDictionary<int, string>(redisClient);
            dictionary.Add(kvp);
            dictionary.Add(kvp1);

            CollectionAssert.AreEquivalent(new[] { 1, 2 }, dictionary.Keys);
        }

        [Test]
        public void Values_AddedValues()
        {
            var kvp = new KeyValuePair<int, string>(1, "val1");
            var kvp1 = new KeyValuePair<int, string>(2, "val2");
            var dictionary = new RedisDictionary<int, string>(redisClient);

            dictionary.Add(kvp);
            dictionary.Add(kvp1);

            CollectionAssert.AreEquivalent(new[] { "val1", "val2" }, dictionary.Values);
        }

        [Test]
        public void Values_ValueValueType_AddedValues()
        {
            var kvp = new KeyValuePair<int, int>(1, 3);
            var kvp1 = new KeyValuePair<int, int>(2, 4);
            var dictionary = new RedisDictionary<int, int>(redisClient);

            dictionary.Add(kvp);
            dictionary.Add(kvp1);

            CollectionAssert.AreEquivalent(new[] { 3, 4 }, dictionary.Values);
        }

        [Test]
        public void Remove_KeyValuePairExists_True()
        {
            var kvp = new KeyValuePair<int, int>(1, 3);
            var dictionary = new RedisDictionary<int, int>(redisClient);

            dictionary.Add(kvp);

            Assert.IsTrue(dictionary.Remove(new KeyValuePair<int, int>(1, 3)));
        }

        [Test]
        public void Remove_WrongKey_False()
        {
            var kvp = new KeyValuePair<int, int>(1, 3);
            var dictionary = new RedisDictionary<int, int>(redisClient);

            dictionary.Add(kvp);

            Assert.IsFalse(dictionary.Remove(new KeyValuePair<int, int>(2, 3)));
        }


        [Test]
        public void Remove_WrongValue_False()
        {
            var kvp = new KeyValuePair<int, int>(1, 3);
            var dictionary = new RedisDictionary<int, int>(redisClient);

            dictionary.Add(kvp);

            Assert.IsFalse(dictionary.Remove(new KeyValuePair<int, int>(1, 4)));
        }


        [Test]
        public void Index_StoredValue()
        {
            var kvp = new KeyValuePair<string, string>("key1", "val1");
            var dictionary = new RedisDictionary<string, string>(redisClient);

            dictionary.Add(kvp);

            Assert.AreEqual("val1", dictionary["key1"]);
        }


        [Test]
        public void Index_ValueType_StoredValue()
        {
            var kvp = new KeyValuePair<int, int>(1, 3);
            var dictionary = new RedisDictionary<int, int>(redisClient);

            dictionary.Add(kvp);

            Assert.AreEqual(3, dictionary[1]);
        }
    }
}