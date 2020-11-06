using Newtonsoft.Json;
using System;
using System.Threading;

namespace Alachisoft.NCache.Data.Caching
{
    public class DataCacheNotificationDescriptor
    {
        [Obsolete("Cache Notifications are only supported in NCache Enterprise AppFabric Wrapper", true)]
        internal DataCacheNotificationDescriptor(
            string cacheName)
        {
            CacheName = cacheName;
            DelegateId = Interlocked.Increment(ref globalDelegateID);
        }

        [Obsolete("Cache Notifications are only supported in NCache Enterprise AppFabric Wrapper", true)]
        internal DataCacheNotificationDescriptor(DataCacheNotificationDescriptor other)
        {
            CacheName = other.CacheName;
            DelegateId = other.DelegateId;
        }
        public string CacheName { get; }
        public long DelegateId { get; }

        private static long globalDelegateID = 0L;

        public override string ToString()
        {
            if (this == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
