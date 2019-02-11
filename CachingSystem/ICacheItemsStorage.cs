using System;

namespace CachingSystem
{
    public interface ICacheItemsStorage
    {
        /// <summary>
        /// The method that adds object to the cache.
        /// </summary>
        /// <param name="key"> The key of the object to be added. </param>
        /// <param name="value"> The object to be added. </param>
        /// <param name="expirationTime"> The expiration time of the object from the cache. </param>
        void Add(string key, object value, TimeSpan expirationTime);

        /// <summary>
        /// The method that gets object from the cache by <paramref name="key"/>
        /// </summary>
        /// <param name="key"> The key by which object is to be gotten. </param>
        /// <returns> The object which is stored in the cache by the <paramref name="key"/>. </returns>
        CacheItem Get(string key);

        /// <summary>
        /// The method that changes value found in cache by the key <paramref name="key"/>.
        /// </summary>
        /// <param name="key"> The key of the value in the cache. </param>
        /// <param name="value"> The new value of the object found by key <paramref name="key"/>. </param>
        /// <param name="expirationTime"> The new expiration time of the object found by key <paramref name="key"/></param>
        void ChangeValue(string key, object value, TimeSpan expirationTime);

        /// <summary>
        /// The method that removes expired objects from the cache.
        /// </summary>
        void ClearExpired();
    }
}