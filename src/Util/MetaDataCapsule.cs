using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alachisoft.NCache.Data.Caching.Util
{
    /// <summary>
    /// Encapsulates value to be inserted into cache.
    /// Introduced to store itemversion
    /// </summary>
    [Serializable]
    class MetaDataCapsule
    {
        private object value;

        public ItemVersion CacheItemVersion { get; set; }
        public string Group { get; set; }
        public object Value { get { return this.value; } }
        
        private MetaDataCapsule(object value, ItemVersion version, string group)
        {
            this.CacheItemVersion = version;
            this.Group = group;
            this.value = value;
        }

        public static MetaDataCapsule Encapsulate(object value, string region)
        {
            return new MetaDataCapsule(value, 1, region);

        }

    }
}
