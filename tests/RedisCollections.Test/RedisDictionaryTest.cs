using System;
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
        public void ShouldAddItem()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" } };
            Assert.AreEqual(dictionary["key1"], "val1");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShoudThrowArgumentExceptionIfAddTheSameKey()
        {

            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" } };
            dictionary.Add("key1", "val2");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShoudThrowArgumentNullExceptionIfKeyIsNull()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { null, "val1" } };
        }

        [Test]
        public void DifferentDictionariesCanHasItemsWithTheSameKey()
        {
            var dictionary1 = new RedisDictionary<string, string>(redisClient) { { "key", "val1" } };
            var dictionary2 = new RedisDictionary<string, string>(redisClient) { { "key", "val2" } };
            Assert.AreNotEqual(dictionary1["key"], dictionary2["key"]);
        }

        [Test]
        public void ShouldReturnItemsCount()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            new RedisDictionary<string, string>(redisClient) { { "key", "val2" } };
            Assert.AreEqual(2, dictionary.Count);
        }

        [Test]
        public void ShouldReturnFalseIfDoesNotContainKey()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            Assert.IsFalse(dictionary.ContainsKey("NotExistingKey"));
        }

        [Test]
        public void ShouldReturnTrueIfContainsKey()
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
        public void ShouldReturnTrueIfCanGetValue()
        {
            var dictionary = new RedisDictionary<string, string>(redisClient) { { "key1", "val1" }, { "key2", "val1" } };
            string res;
            Assert.IsTrue(dictionary.TryGetValue("key1", out res));
            Assert.AreEqual("val1", res);
        }
    }
}
