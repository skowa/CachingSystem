using System;

namespace CachingSystem
{
    /// <summary>
    /// The class of cache item.
    /// </summary>
    public class CacheItem
    {
        private readonly DateTime _expirationDateTime;

        /// <summary>
        /// Initializes a new instance of cache item.
        /// </summary>
        /// <param name="value">The value of cache item</param>
        /// <param name="expirationDateTime">The expiration time of item.</param>
        public CacheItem(object value, DateTime expirationDateTime)
        {
            Value = value;
            _expirationDateTime = expirationDateTime;
        }

        /// <summary>
        /// Gets the value of cache item.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Checks whether the cache item is alive on not.
        /// </summary>
        public bool IsAlive => _expirationDateTime > DateTime.UtcNow;
    }
}