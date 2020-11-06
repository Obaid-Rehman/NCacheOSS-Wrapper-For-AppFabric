using Newtonsoft.Json;
using System;

namespace Alachisoft.NCache.Data.Caching
{
    public class BaseOperationNotification
    {
        [Obsolete("Cache Notifications are only supported in NCache Enterprise AppFabric Wrapper", true)]
        [JsonConstructor]
        public BaseOperationNotification(string cacheName, DataCacheOperations opType, DataCacheItemVersion version)
        {
            CacheName = cacheName;
            OperationType = opType;
            Version = version;
        }

        public string CacheName { get; }
        public DataCacheOperations OperationType { get; }
        public DataCacheItemVersion Version { get; }

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
