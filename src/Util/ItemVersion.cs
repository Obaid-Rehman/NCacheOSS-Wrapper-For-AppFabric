using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alachisoft.NCache.Data.Caching.Util
{
    [Serializable]
    internal class ItemVersion
    {

        public long Version { get; set; }

        public ItemVersion(long version)
        {
            this.Version = version;
        }

        public static implicit operator ItemVersion(long version)
        {
            return new ItemVersion(version);
        }

        public static implicit operator long(ItemVersion version)
        {
            return version.Version;
        }
    }
}
