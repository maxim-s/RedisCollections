using System;
using System.Linq;
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

            Assert.AreEqual("item1", list[0]);
        }

        [Test]
        public void Clear_AllItemsAreRemoved()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");
            list.Add("item2");
            list.Clear();

            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Contains_EmptyList_False()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            Assert.IsFalse(list.Contains("item"));
        }

        [Test]
        public void Contains_ListSearchedTheSameItem_True()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item");

            Assert.IsTrue(list.Contains("item"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_ArrayLengthLessThanAvailableItemsCount_ArgumentExceptionThrown()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");
            list.Add("item2");
            list.CopyTo(new string[1], 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_IndexGreaterThanArrayLength_ArgumentExceptionThrown()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");
            list.Add("item2");
            list.CopyTo(new string[2], 4);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyTo_NegativeAsIndexParam_ArgumentExceptionThrown()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");
            list.Add("item2");
            list.CopyTo(new string[2], -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyTo_NullAsArrayParam_ArgumentNullExceptionThrown()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");
            list.Add("item2");
            list.CopyTo(null, 0);
        }

        [Test]
        public void CopyTo_SimpleValues_Copied()
        {
            var list = redisCollectionsManager.GetList<string>("list");
            list.Add("item1");
            list.Add("item2");
            var strings = new string[2];
            list.CopyTo(strings, 0);
            Assert.IsTrue(strings.Contains("item1"));
            Assert.IsTrue(strings.Contains("item2"));
        }

        [Test]
        public void Remove_ItemExists_True()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            list.Add("item");

            Assert.IsTrue(list.Remove("item"));
        }

        [Test]
        public void Remove_WrongItem_False()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            list.Add("item");

            Assert.IsFalse(list.Remove("SomeWrongItem"));
        }

        [Test]
        public void Count_EmptyList_Zero()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Count_NotEmptyList_NumberOfElements()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            list.Add("item1");
            list.Add("item2");

            Assert.AreEqual(2, list.Count);
        }


        [Test]
        public void IndexOf_EmptyList_Negative()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            Assert.IsTrue(list.IndexOf("item1") < 0);
        }

        [Test]
        public void IndexOf_ItemExists_IndexOfItem()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            list.Add("item1");
            list.Add("item2");
            list.Add("item3");

            Assert.AreEqual(1, list.IndexOf("item2"));
        }

        [Test]
        public void IndexOf_UnexsistingItem_Negative()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            list.Add("item1");
            list.Add("item2");
            list.Add("item3");

            Assert.IsTrue(list.IndexOf("item4") < 0);
        }

        [Test]
        [Ignore]
        public void Insert_Item_Inserted()
        {
            var list = redisCollectionsManager.GetList<string>("list");

            list.Insert(0, "item1");

            Assert.AreEqual(0, list.IndexOf("item1"));
        }
    }
}
