using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Alachisoft.NCache.Data.Caching
{
    internal static class DataFormatter
    {
        internal static CacheItem CreateCacheItem(string key, string cacheName, object value, IEnumerable<DataCacheTag> tags, bool expirable, TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
            }



            var dataCacheItem = new DataCacheItem
            {
                Value = value,
                RegionName = "Default_Region",
                Version = new DataCacheItemVersion(),
                CacheName = cacheName,
                Key = key.Trim()
            };

            if (tags != null && tags.Count() > 0)
            {
                dataCacheItem.Tags = new ReadOnlyCollection<DataCacheTag>(tags.ToList());
            }

            

            if (expirable)
            {
                dataCacheItem.Timeout = timeout;
            }
            else
            {
                dataCacheItem.Timeout = TimeSpan.MaxValue;
            }

            var cacheItem = new CacheItem(dataCacheItem);

            if (expirable)
            {
                cacheItem.Expiration = new Expiration(ExpirationType.Absolute, timeout);
            }

            return cacheItem;
        }

    }
}
