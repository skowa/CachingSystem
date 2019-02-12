using System;
using System.Threading;
using NUnit.Framework;

namespace CachingSystem.Tests
{
    [TestFixture]
    public class CacheItemTests
    {
        [TestCase(5)]
        [TestCase("hello")]
        public void ValueProperty_AnInstanceWithValueAndExpirationTime_TheValueReturned(object value)
        {
            CacheItem cacheItem = new CacheItem(value, DateTime.UtcNow + new TimeSpan(1000));

            Assert.AreEqual(value, cacheItem.Value);
        }

        [Test]
        public void IsAliveProperty_AnInstanceNotAlive_ReturnFalse()
        {
            CacheItem cacheItem = new CacheItem(5, DateTime.UtcNow + new TimeSpan(0, 0, 1));

            Thread.Sleep(1001);

            Assert.IsFalse(cacheItem.IsAlive);
        }

        [Test]
        public void IsAliveProperty_AnInstanceAlive_ReturnTrue()
        {
            CacheItem cacheItem = new CacheItem(5, DateTime.UtcNow + new TimeSpan(0, 0, 1));

            Assert.IsTrue(cacheItem.IsAlive);
        }
    }
}