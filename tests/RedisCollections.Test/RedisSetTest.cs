using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using NUnit.Framework;
using RedisCollections.Client;
using IRedisClient = RedisCollections.Client.IRedisClient;


namespace RedisCollections.Test
{
    [TestFixture]
    class RedisSetTest
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
            redisClient = new Redis(new RedisClient("localhost"));
            redisCollectionsManager = new RedisCollectionsManager(redisClient);
        }

        [TestFixtureTearDown]
        public static void Destroy()
        {
            redisClient.Dispose();
        }

        [Test]
        public void Add_AddExistingItem_False()
        {

            ISet<string> set = redisCollectionsManager.GetSet<string>();
            set.Add("val1");
            Assert.IsFalse(set.Add("val1"));
        }

        [Test]
        public void Add_Item_ItemAdded()
        {
            ISet<string> set = redisCollectionsManager.GetSet<string>();
            Assert.IsTrue(set.Add("val1"));
        }
    }
}
