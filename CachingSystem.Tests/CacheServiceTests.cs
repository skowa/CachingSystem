using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using CachingSystem;
using Moq;

namespace Tests
{
    public class Tests
    {
        private Mock<IDictionary<string, object>> _cacheMock;

        [SetUp]
        public void Setup()
        {
            _cacheMock = new Mock<IDictionary<string, object>>();
        }

        #region Add method

        [Test]
        public void Add_KeyValueAndExpirationTimeAreValid_ObjectIsAddedToTheCache()
        {
            string key = "Some key";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheService = new CacheService();
            cacheService.Add(key, value, expirationTime);

            Assert.AreEqual(value, cacheService.Get(key));
        }

        [Test]
        public void Add_AddObjectsOfDifferentTypesToTheCache_ObjectsAreAddedToTheCache()
        {
            string keyOfString = "String key";
            string valueOfString = "Some value";

            string keyOfInt = "Int key";
            int valueOfInt = 100;

            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheService = new CacheService();
            cacheService.Add(keyOfString, valueOfString, expirationTime);
            cacheService.Add(keyOfInt, valueOfInt, expirationTime);

            Assert.AreEqual(valueOfString, cacheService.Get(keyOfString));
            Assert.AreEqual(valueOfInt, cacheService.Get(keyOfInt));
        }

        [Test]
        public void Add_KeyIsTheSameAsTheKeyOfTheExpiredObject_ObjectIsAddedToTheCache()
        {
            string key = "Some key";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 10);

            CacheService cacheService = new CacheService(
                new Dictionary<string, object>
                {
                    { key, "sss" },
                    { "Other key", "Other value" }
                },
                new TimeSpan(0, 0, 1));

            Thread.Sleep(2000);
            cacheService.Add(key, value, expirationTime);
            Assert.AreEqual(value, cacheService.Get(key));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Add_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key)
        {
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentException>(() => new CacheService().Add(key, value, expirationTime));
        }

        [Test]
        public void Add_ValueIsNull_ThrownArgumentNullException()
        {
            string key = "Some key";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentNullException>(() => new CacheService().Add(key, null, expirationTime));
        }

        [Test]
        public void Add_KeyAlreadyExists_ThrownInvalidOperationException()
        {
            string repeatedKey = "Key is repeated";

            CacheService cacheService = new CacheService(
                new Dictionary<string, object>
                {
                    {repeatedKey, "sss"},
                    { "Other key", 2323 }
                },
                new TimeSpan(0, 30, 0));

            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<InvalidOperationException>(() => cacheService.Add(repeatedKey, value, expirationTime));
        }

        #endregion

        #region Get method

        [Test]
        public void Get_KeyOfTheObjectFromTheCache_ObjectReturns()
        {
            string key = "Some key";
            int expected = 100;

            CacheService cacheService = new CacheService(
                new Dictionary<string, object>
                {
                    { "Other key", 233 },
                    { key, expected },
                    { "Other-other key", "Other-other value" }
                },
                new TimeSpan(0, 0, 1));

            int actual = (int)cacheService.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Get_KeyOfTheExpiredObjectFromTheCache_ThrownInvalidOperationException()
        {
            string key = "Some key";

            CacheService cacheService = new CacheService(
                new Dictionary<string, object>
                {
                    { "Other key", 233 },
                    { key, 100 },
                    { "Other-other key", "Other-other value" }
                },
                new TimeSpan(0, 0, 1));

            Thread.Sleep(1005);

            Assert.Throws<InvalidOperationException>(() => cacheService.Get(key));
        }

        [Test]
        public void Get_KeyIsNotInTheCache_ThrownInvalidOperationException()
        {
            string key = "Some key";

            CacheService cacheService = new CacheService(
                new Dictionary<string, object>
                {
                    { "Other key", 233 },
                    { "Other-other key", "Other-other value" }
                },
                new TimeSpan(0, 0, 1));

            Assert.Throws<InvalidOperationException>(() => cacheService.Get(key));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Get_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key) =>
            Assert.Throws<ArgumentException>(() => new CacheService().Get(key));

        #endregion
    }
}