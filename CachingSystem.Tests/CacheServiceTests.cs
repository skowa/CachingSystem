using System;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace CachingSystem.Tests
{
    public class CacheServiceTests
    {
        private Mock<ICacheItemsStorage> _cacheMock;

        [SetUp]
        public void Setup()
        {
            _cacheMock = new Mock<ICacheItemsStorage>();
        }

        #region ctor

        [Test]
        public void Ctor_ICacheItemsStorageIsNull_ThrownArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => new CacheService(null));

        #endregion

        #region Add method

        [Test]
        public void Add_KeyValueAndExpirationTimeAreValid_GetMethodCalled()
        {
            string key = "Some key";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheService = new CacheService(_cacheMock.Object);
            cacheService.Add(key, value, expirationTime);

            _cacheMock.Verify(cache =>
                cache.Get(It.Is<string>(s => s == key)));
        }

        [Test]
        public void Add_KeyValueAndExpirationTimeAreValid_AddMethodCalled()
        {
            string key = "Some key";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheService = new CacheService(_cacheMock.Object);
            cacheService.Add(key, value, expirationTime);

            _cacheMock.Verify(cache =>
                cache.Add(It.Is<string>(s => s == key), It.Is<string>(o => o == value), expirationTime));
        }

        [Test]
        public void Add_AddObjectsOfDifferentTypesToTheCache_AddMethodIsCalled()
        {
            string keyOfString = "String key";
            string valueOfString = "Some value";

            string keyOfInt = "Int key";
            int valueOfInt = 100;

            var expirationTime = new TimeSpan(0, 0, 30);

            var cacheService = new CacheService(_cacheMock.Object);
            cacheService.Add(keyOfString, valueOfString, expirationTime);
            cacheService.Add(keyOfInt, valueOfInt, expirationTime);

            _cacheMock.Verify(cache =>
                cache.Add(It.Is<string>(s => s == keyOfString), It.Is<string>(o => o == valueOfString), expirationTime));
            _cacheMock.Verify(cache =>
                cache.Add(It.Is<string>(s => s == keyOfInt), It.Is<int>(o => o == valueOfInt), expirationTime));
        }

        [Test]
        public void Add_KeyIsTheSameAsTheKeyOfTheExpiredObject_ChangeValueCalled()
        {
            string key = "Some key";
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 10);

            _cacheMock.Setup(cache => cache.Get(key))
                .Returns(new CacheItem(value, DateTime.UtcNow + new TimeSpan(0, 0, 1)));
            
            CacheService cacheService = new CacheService(_cacheMock.Object);

            Thread.Sleep(2000);

            cacheService.Add(key, value, expirationTime);
            _cacheMock.Verify(cache => cache.ChangeValue(It.Is<string>(s => s == key), It.Is<string>(s => s == value), It.IsAny<TimeSpan>()));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Add_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key)
        {
            string value = "Some value";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentException>(() => new CacheService(_cacheMock.Object).Add(key, value, expirationTime));
        }

        [Test]
        public void Add_ValueIsNull_ThrownArgumentNullException()
        {
            string key = "Some key";
            var expirationTime = new TimeSpan(0, 0, 30);

            Assert.Throws<ArgumentNullException>(() => new CacheService(_cacheMock.Object).Add(key, null, expirationTime));
        }

        [Test]
        public void Add_KeyAlreadyExists_ThrownInvalidOperationException()
        {
            string repeatedKey = "Key is repeated";
            string value = "Some value";
            TimeSpan expirationTime = new TimeSpan(1000);

            _cacheMock.Setup(cache => cache.Get(repeatedKey))
                .Returns(new CacheItem("cacheItem", DateTime.UtcNow + new TimeSpan(0, 0, 10)));

            CacheService cacheService = new CacheService(_cacheMock.Object);

            Assert.Throws<InvalidOperationException>(() => cacheService.Add(repeatedKey, value, expirationTime));
        }

        #endregion

        #region Get method

        [Test]
        public void Get_KeyOfTheObjectFromTheCache_GetCalled()
        {
            string key = "Some key";

            _cacheMock.Setup(cache => cache.Get(key))
                .Returns(new CacheItem("SomeValue", DateTime.UtcNow + new TimeSpan(0, 0, 1)));

            CacheService cacheService = new CacheService(_cacheMock.Object);
            cacheService.Get(key);

            _cacheMock.Verify(cache => cache.Get(It.Is<string>(s => s == key)));
        }

        [Test]
        public void Get_KeyOfTheObjectFromTheCache_ObjectReturns()
        {
            string key = "Some key";
            int expected = 100;

            _cacheMock.Setup(cache => cache.Get(key))
                .Returns(new CacheItem(expected, DateTime.UtcNow + new TimeSpan(0, 0, 1)));

            CacheService cacheService = new CacheService(_cacheMock.Object);
            int actual = (int)cacheService.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Get_KeyOfTheExpiredObjectFromTheCache_ThrownInvalidOperationException()
        {
            string key = "Some key";

            _cacheMock.Setup(cache => cache.Get(key))
                .Returns(new CacheItem("Some value", DateTime.UtcNow + new TimeSpan(0, 0, 1)));
            CacheService cacheService = new CacheService(_cacheMock.Object);

            Thread.Sleep(1005);

            Assert.Throws<InvalidOperationException>(() => cacheService.Get(key));
        }

        [Test]
        public void Get_KeyIsNotInTheCache_ThrownInvalidOperationException()
        {
            string key = "Some key";

            CacheService cacheService = new CacheService(_cacheMock.Object);

            Assert.Throws<InvalidOperationException>(() => cacheService.Get(key));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Get_KeyIsNullOrWhiteSpace_ThrownArgumentException(string key) =>
            Assert.Throws<ArgumentException>(() => new CacheService(_cacheMock.Object).Get(key));

        #endregion
    }
}