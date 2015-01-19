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
    }
}
