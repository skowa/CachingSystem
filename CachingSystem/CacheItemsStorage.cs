using System;
using System.Collections.Generic;
using System.Linq;

namespace CachingSystem
{
    public class CacheItemsStorage : ICacheItemsStorage
    {
        private readonly IDictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();

        /// <summary>
        /// The method that adds object to the cache.
        /// </summary>
        /// <param name="key"> The key of the object to be added. </param>
        /// <param name="value"> The object to be added. </param>
        /// <param name="expirationTime"> The expiration time of the object from the cache. </param>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="value"/> is null. </exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or is white space. </exception>
        public void Add(string key, object value, TimeSpan expirationTime)
        {
            CheckArguments(key, value);

            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, new CacheItem(value, DateTime.UtcNow + expirationTime));
            }
            else
            {
                throw new InvalidOperationException($"The value by key {key} exists");
            }
        }

        /// <summary>
        /// The method that changes value found in cache by the key <paramref name="key"/>.
        /// </summary>
        /// <param name="key"> The key of the value in the cache. </param>
        /// <param name="value"> The new value of the object found by key <paramref name="key"/>. </param>
        /// <param name="expirationTime"> The new expiration time of the object found by key <paramref name="key"/></param>
        /// <exception cref="InvalidOperationException"> Thrown when there is no key <paramref name="key"/> in the cache. </exception>
        public void ChangeValue(string key, object value, TimeSpan expirationTime)
        {
            CheckArguments(key, value);

            if (_cache.ContainsKey(key))
            {
                _cache[key] = new CacheItem(value, DateTime.UtcNow + expirationTime);
            }
            else
            {
                throw new InvalidOperationException($"The value by key {key} is not found");
            }
        }

        /// <summary>
        /// The method that removes expired objects from the cache.
        /// </summary>
        public void ClearExpired()
        {
            List<string> keysToRemove = _cache.Where(pair => !pair.Value.IsAlive).Select(pair => pair.Key).ToList();

            foreach (string key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// The method that gets object from the cache by <paramref name="key"/>
        /// </summary>
        /// <param name="key"> The key by which object is to be gotten. </param>
        /// <returns> The object which is stored in the cache by the <paramref name="key"/>. </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> is null or is white space. </exception>
        public CacheItem Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{nameof(key)} is invalid");
            }

            _cache.TryGetValue(key, out CacheItem cacheItem);

            return cacheItem;
        }

        private void CheckArguments(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"{nameof(key)} is invalid");
            }

            if (value == null)
            {
                throw new ArgumentNullException($"{nameof(value)} is null");
            }
        }
    }
}