using System.Collections.Generic;
using NUnit.Framework;
using RedisCollections.Client;

namespace RedisCollections.Test
{
    [TestFixture]
    public class RedisDictionaryIEnumeratorTest
    {
        [SetUp]
        public void Before()
        {
            redisClient.FlushAll();
        }

        private static RedisCollectionsManager redisCollectionsManager;
        private static IRedisClient redisClient;

        [TestFixtureSetUp]
        public void Init()
        {
            //redisClient = new RedisClient();
            redisCollectionsManager = new RedisCollectionsManager(redisClient);
        }

        [TestFixtureTearDown]
        public static void Destroy()
        {
            redisClient.Dispose();
        }

        [Test]
        public void Current_InvokeMoveNext1Time_Returned()
        {
            IDictionary<string, string> dictionary = redisCollectionsManager.GetDictionary<string, string>();
            dictionary.Add("key2", "val2");
            dictionary.Add("key1", "val1");
            IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
            enumerator.MoveNext();
            Assert.IsTrue(dictionary.Contains(enumerator.Current));
        }

        [Test]
        public void MoveNext_EmptyDictionary_False()
        {
            IDictionary<string, string> dictionary = redisCollectionsManager.GetDictionary<string, string>();
            Assert.IsFalse(dictionary.GetEnumerator().MoveNext());
        }


        [Test]
        public void MoveNext_InvokeCountTimes_True()
        {
            IDictionary<string, string> dictionary = redisCollectionsManager.GetDictionary<string, string>();
            dictionary.Add("key2", "val2");
            dictionary.Add("key1", "val1");
            IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.MoveNext());
        }

        [Test]
        public void MoveNext_InvokeMoreThanItemsCount_False()
        {
            IDictionary<string, string> dictionary = redisCollectionsManager.GetDictionary<string, string>();
            dictionary.Add("key2", "val2");
            dictionary.Add("key1", "val1");
            IEnumerator<KeyValuePair<string, string>> enumerator = dictionary.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}