using System;
using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack.Redis;

namespace RedisCollections.Test
{
    [TestFixture]
    public class RedisDictionaryIEnumeratorTest
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
        public void Current_InvokeMoveNext1Time_Returned()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val2" } };
            var enumerator = dictionary.GetEnumerator();
            enumerator.MoveNext();
            Assert.IsTrue(dictionary.Contains(enumerator.Current));

        }

        [Test]
        public void MoveNext_EmptyDictionary_False()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient);
            Assert.IsFalse(dictionary.GetEnumerator().MoveNext());
        }


        [Test]
        public void MoveNext_InvokeCountTimes_True()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val2" } };
            var enumerator = dictionary.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
        }

        [Test]
        public void MoveNext_InvokeMoreThanItemsCount_False()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, {"key2","val2"} };
            var enumerator = dictionary.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}
