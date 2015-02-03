using CSRedis;
using NUnit.Framework;
using RedisCollections.Client;
using IRedisClient = RedisCollections.Client.IRedisClient;

namespace RedisCollections.Test
{
    [TestFixture]
    class RedisListTest
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
        public void Add_ItemAdded()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");

            Assert.IsTrue(list.Contains("item1"));
        }
    }
}
