using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alachisoft.NCache.Data.Caching
{

    [Flags]
    public enum DataCacheOperations
    {
        AddItem = 1,
        ReplaceItem = 2,
        RemoveItem = 4,
        [Obsolete("Region Callback is only available ", true)]
        CreateRegion = 8,
        [Obsolete("Region Callbacks are an Enterprise Feature", true)]
        RemoveRegion = 16,
        [Obsolete("Region Callbacks are an Enterprise Feature", true)]
        ClearRegion = 32,
    }
}
