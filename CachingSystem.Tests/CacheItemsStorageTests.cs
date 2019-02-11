using System;
using System.Threading;
using NUnit.Framework;

namespace CachingSystem.Tests
{
    [TestFixture]
    public class CacheItemsStorageTests
    {
        #region Add method

        [Test]
        public void Add_KeyValueAndExpirationTimeAreValid_ObjectIsAddedToTheCache()
        {
            string key = "Some key";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheItemsStorage = new CacheItemsStorage();
            cacheItemsStorage.Add(key, value, expirationTime);

            Assert.AreEqual(value, cacheItemsStorage.Get(key).Value);
        }

        [Test]
        public void Add_AddObjectsOfDifferentTypesToTheCache_ObjectsAreAddedToTheCache()
        {
            string keyOfString = "String key";
            string valueOfString = "Some value";

            string keyOfInt = "Int key";
            int valueOfInt = 100;

            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheItemsStorage = new CacheItemsStorage();
            cacheItemsStorage.Add(keyOfString, valueOfString, expirationTime);
            cacheItemsStorage.Add(keyOfInt, valueOfInt, expirationTime);

            Assert.AreEqual(valueOfString, cacheItemsStorage.Get(keyOfString).Value);
            Assert.AreEqual(valueOfInt, cacheItemsStorage.Get(keyOfInt).Value);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Add_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key)
        {
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentException>(() => new CacheItemsStorage().Add(key, value, expirationTime));
        }

        [Test]
        public void Add_ValueIsNull_ThrownArgumentNullException()
        {
            string key = "Some key";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentNullException>(() => new CacheItemsStorage().Add(key, null, expirationTime));
        }

        [Test]
        public void Add_KeyAlreadyExists_ThrownInvalidOperationException()
        {
            string repeatedKey = "Key is repeated";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            CacheItemsStorage cacheItemsStorage = new CacheItemsStorage();
            cacheItemsStorage.Add(repeatedKey, value, expirationTime);

            Assert.Throws<InvalidOperationException>(() => cacheItemsStorage.Add(repeatedKey, value, expirationTime));
        }

        #endregion

        #region ChangeValue method

        [Test]
        public void ChangeValue_KeyValueAndExpirationTimeAreValid_ObjectIsUpdatedInTheCache()
        {
            string key = "Some key";
            string value = "Some value";
            string newValue = "New value";
            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheItemsStorage = new CacheItemsStorage();
            cacheItemsStorage.Add(key, value, expirationTime);

            cacheItemsStorage.ChangeValue(key, newValue, expirationTime);
            Assert.AreEqual(newValue, cacheItemsStorage.Get(key).Value);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void ChangeValue_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key)
        {
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentException>(() => new CacheItemsStorage().ChangeValue(key, value, expirationTime));
        }

        [Test]
        public void ChangeValue_ValueIsNull_ThrownArgumentNullException()
        {
            string key = "Some key";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentNullException>(() => new CacheItemsStorage().ChangeValue(key, null, expirationTime));
        }

        [Test]
        public void ChangeValue_KeyAlreadyExists_ThrownInvalidOperationException()
        {
            string repeatedKey = "Key is repeated";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            CacheItemsStorage cacheItemsStorage = new CacheItemsStorage();

            Assert.Throws<InvalidOperationException>(() => cacheItemsStorage.ChangeValue(repeatedKey, value, expirationTime));
        }
        #endregion

        #region Get method

        [Test]
        public void Get_KeyOfTheObjectFromTheCache_ObjectReturns()
        {
            string key = "Some key";
            int expected = 100;

            CacheItemsStorage cacheItemsStorage = new CacheItemsStorage();
            cacheItemsStorage.Add(key, expected, new TimeSpan(0, 10, 0));

            CacheItem actual = cacheItemsStorage.Get(key);

            Assert.AreEqual(expected, actual.Value);
        }

        [Test]
        public void Get_KeyIsNotInTheStorage_ReturnsNull()
        {
            CacheItemsStorage cacheItemsStorage = new CacheItemsStorage();

            CacheItem actual = cacheItemsStorage.Get("Some key");

            Assert.AreEqual(null, actual);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Get_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key) =>
            Assert.Throws<ArgumentException>(() => new CacheItemsStorage().Get(key));

        #endregion

        #region ClearExpired method

        [Test]
        public void ClearExpired_SomeObjectsInTheCacheAndSomeOfThemAreExpired_ExpiredObjectsAreRemovedFromTheCache()
        {
            string key1 = "key1";
            string key2 = "key2";
            string key3 = "key3";
            string key4 = "key4";

            CacheItemsStorage cacheItemsStorage = new CacheItemsStorage();
            cacheItemsStorage.Add(key1, "value1", new TimeSpan(0, 0, 1));
            cacheItemsStorage.Add(key2, "value2", new TimeSpan(0, 0, 2));
            cacheItemsStorage.Add(key3, "value3", new TimeSpan(0, 0, 10));
            cacheItemsStorage.Add(key4, "value4", new TimeSpan(0, 0, 1));

            Thread.Sleep(2000);
            cacheItemsStorage.ClearExpired();
            Assert.AreEqual("value3", cacheItemsStorage.Get(key3).Value);
            Assert.AreEqual(null, cacheItemsStorage.Get(key1));
            Assert.AreEqual(null, cacheItemsStorage.Get(key2));
            Assert.AreEqual(null, cacheItemsStorage.Get(key4));
        }

        #endregion
    }
}