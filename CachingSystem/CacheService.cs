using System;
using System.Threading;

namespace CachingSystem
{
    /// <summary>
    /// The class of cache service.
    /// </summary>
    public class CacheService
    {
        private readonly ICacheItemsStorage _cache;

        private readonly Timer _timer;

        private readonly int _periodOfChecking = 30000;

        /// <summary>
        /// Initializes a new instance of <see cref="CacheService"/>.
        /// </summary>
        /// <param name="cache"> The items to be stored in the cache. </param>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="cache"/> is null. </exception>
        public CacheService(ICacheItemsStorage cache)
        {
            _cache = cache ?? throw new ArgumentNullException($"{nameof(cache)} is null");
            _timer = new Timer(o => OnTimeIsOver(), null, 0, _periodOfChecking);
        }

        /// <summary>
        /// The method that adds object to the cache.
        /// </summary>
        /// <param name="key"> The key of the object to be added. </param>
        /// <param name="value"> The object to be added. </param>
        /// <param name="expirationTime"> The expiration time of the object from the cache. </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <paramref name="key"/> is already used for storing an object.
        /// </exception>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="value"/> is null. </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or is white space. </exception>
        public void Add(string key, object value, TimeSpan expirationTime)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{nameof(key)} is invalid");
            }

            if (value == null)
            {
                throw new ArgumentNullException($"{nameof(value)} is null");
            }

            CacheItem cacheItem = _cache.Get(key);
            if (cacheItem != null)
            {
                if (cacheItem.IsAlive)
                {
                    throw new InvalidOperationException($"The key {key} is already in the cache");
                }

                _cache.ChangeValue(key, value, expirationTime);
                return;
            }
            
            _cache.Add(key, value, expirationTime);
        }

        /// <summary>
        /// The method that gets object from the cache by <paramref name="key"/>
        /// </summary>
        /// <param name="key"> The key by which object is to be gotten. </param>
        /// <returns> The object which is stored in the cache by the <paramref name="key"/>. </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or is white space. </exception>
        /// <exception cref="InvalidOperationException">Thrown when object is not found by the <paramref name="key"/>. </exception>
        public object Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{nameof(key)} is invalid");
            }

            CacheItem cacheItem = _cache.Get(key);

            return cacheItem != null && cacheItem.IsAlive
                ? cacheItem.Value
                : throw new InvalidOperationException($"The item with key {key} does not exist");
        }

        private void OnTimeIsOver() => _cache.ClearExpired();
    }
}