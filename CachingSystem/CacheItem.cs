using System;

namespace CachingSystem
{
    internal class CacheItem
    {
        private readonly DateTime _expirationDateTime;

        internal CacheItem(object value, DateTime expirationDateTime)
        {
            Value = value;
            _expirationDateTime = expirationDateTime;
        }

        internal object Value { get; }

        internal bool IsAlive => _expirationDateTime > DateTime.UtcNow;
    }
}