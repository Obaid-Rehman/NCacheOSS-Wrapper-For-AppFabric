using System;

namespace Alachisoft.NCache.Data.Caching
{
    [Serializable]
    public class DataCacheItemVersion : IComparable<DataCacheItemVersion>
    {
        internal DataCacheItemVersion()
        {
            CreationTime = DateTime.UtcNow;
        }

        internal DataCacheItemVersion(DataCacheItemVersion other)
        {
            CreationTime = other.CreationTime;
        }

        internal DataCacheItemVersion(DataCacheItem dataCacheItem)
        {
            CreationTime = dataCacheItem.Version.CreationTime;
        }

        internal DateTime CreationTime { get; }

        public static bool operator ==(DataCacheItemVersion left, DataCacheItemVersion right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right))
            {
                return true;
            }
            else if (!ReferenceEquals(null, left) && ReferenceEquals(null, right))
            {
                return false;
            }
            else if (ReferenceEquals(null, left) && !ReferenceEquals(null, right))
            {
                return false;
            }
            else
            {
                return left.CreationTime == right.CreationTime;
            }
        }

        public static bool operator <(DataCacheItemVersion left, DataCacheItemVersion right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right))
            {
                return false;
            }
            else if (ReferenceEquals(null, left) && !ReferenceEquals(null, right))
            {
                return true;
            }
            else if (!ReferenceEquals(null, left) && ReferenceEquals(null, right))
            {
                return false;
            }
            else
            {
                return left.CreationTime < right.CreationTime;
            }
        }

        public static bool operator !=(DataCacheItemVersion left, DataCacheItemVersion right)
        {
            return !(left == right);
        }

        public static bool operator >(DataCacheItemVersion left, DataCacheItemVersion right)
        {
            if (ReferenceEquals(null, left) && ReferenceEquals(null, right))
            {
                return false;
            }
            else if (ReferenceEquals(null, left) && !ReferenceEquals(null, right))
            {
                return false;
            }
            else if (!ReferenceEquals(null, left) && ReferenceEquals(null, right))
            {
                return true;
            }
            else
            {
                return left.CreationTime > right.CreationTime;
            }
        }

        public override bool Equals(object obj)
        {
            DataCacheItemVersion other = obj as DataCacheItemVersion;
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (this == null)
                {
                    return base.GetHashCode();
                }
                else
                {
                    return ("DataCacheItemVersion" + CreationTime.ToString()).GetHashCode();
                }
            }
        }
        public int CompareTo(DataCacheItemVersion other)
        {
            if (this == other)
            {
                return 0;
            }
            else if (this < other)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
