using Alachisoft.NCache.Client;
using Alachisoft.NCache.Common.ErrorHandling;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alachisoft.NCache.Data.Caching
{
    internal class CacheHandler
    {
        private readonly ICache _cache;
        private readonly TimeSpan _defaultTimeout;
        private readonly bool _expirable;
        private readonly string _cacheName;

        internal CacheHandler(string cacheName, CacheConnectionOptions connectionOptions, bool expirable, TimeSpan defaultTimeout)
        {
            _cacheName = cacheName.Trim();
            _expirable = expirable;

            _defaultTimeout = defaultTimeout;

            if (connectionOptions == null)
            {
                _cache = CacheManager.GetCache(cacheName.Trim());
            }
            else
            {
                _cache = CacheManager.GetCache(cacheName.Trim(), connectionOptions);
            }
        }

        internal object Get(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                var dataCacheItem = _cache.Get<DataCacheItem>(key.Trim());

                if (dataCacheItem == null)
                {
                    return null;
                }

                return dataCacheItem.Value;
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal object Get(string key, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal object Get(string key, out DataCacheItemVersion version)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                version = null;

                var dataCacheItem = _cache.Get<DataCacheItem>(key.Trim());

                if (dataCacheItem == null)
                {
                    return null;
                }
                else
                {
                    version = new DataCacheItemVersion(dataCacheItem);
                    return dataCacheItem.Value;
                }
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal object Get(string key, out DataCacheItemVersion version, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }




        internal DataCacheItemVersion Add(string key, object value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }


                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, null, _expirable, _defaultTimeout);

                var cacheKey = key.Trim();

                _cache.Add(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.KEY_ALREADY_EXISTS)
                    {
                        throw new DataCacheException("Key already exists.", ex)
                        {
                            ErrorCode = DataCacheErrorCode.KeyAlreadyExists
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal DataCacheItemVersion Add(string key, object value, IEnumerable<DataCacheTag> tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (tags == null)
                {
                    throw new ArgumentNullException(nameof(tags), "Value cannot be null.");
                }

                if (tags.Count() == 0 || tags.Any(x => x == null))
                {
                    throw new ArgumentException("Either the collection passed is empty or one of the tags passed is null.", nameof(tags));
                }

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, tags, _expirable, _defaultTimeout);

                var cacheKey = key.Trim();

                _cache.Add(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.KEY_ALREADY_EXISTS)
                    {
                        throw new DataCacheException("Key already exists.", ex)
                        {
                            ErrorCode = DataCacheErrorCode.KeyAlreadyExists
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal DataCacheItemVersion Add(string key, object value, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Add(string key, object value, TimeSpan timeout)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, null, _expirable, timeout);

                var cacheKey = key.Trim();

                _cache.Add(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.KEY_ALREADY_EXISTS)
                    {
                        throw new DataCacheException("Key already exists.", ex)
                        {
                            ErrorCode = DataCacheErrorCode.KeyAlreadyExists
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItemVersion Add(string key, object value, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (tags == null)
                {
                    throw new ArgumentNullException(nameof(tags), "Value cannot be null.");
                }

                if (tags.Count() == 0 || tags.Any(x => x == null))
                {
                    throw new ArgumentException("Either the collection passed is empty or one of the tags passed is null.", nameof(tags));
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, tags, _expirable, timeout);

                var cacheKey = key.Trim();

                _cache.Add(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.KEY_ALREADY_EXISTS)
                    {
                        throw new DataCacheException("Key already exists.", ex)
                        {
                            ErrorCode = DataCacheErrorCode.KeyAlreadyExists
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItemVersion Add(string key, object value, TimeSpan timeout, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        
        internal DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal DataCacheItemVersion Put(string key, object value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, null, _expirable, _defaultTimeout);

                var cacheKey = key.Trim();

                _cache.Insert(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency is only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, IEnumerable<DataCacheTag> tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (tags == null)
                {
                    throw new ArgumentNullException(nameof(tags), "Value cannot be null.");
                }

                if (tags.Count() == 0 || tags.Any(x => x == null))
                {
                    throw new ArgumentException("Either the collection passed is empty or one of the tags passed is null.", nameof(tags));
                }

                var cacheKey = key.Trim();

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, tags, _expirable, _defaultTimeout);

                _cache.Insert(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItemVersion Put(string key, object value, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, TimeSpan timeout)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheKey = key.Trim();

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, null, _expirable, timeout);

                _cache.Insert(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, IEnumerable<DataCacheTag> tags)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency is only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency and region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, TimeSpan timeout)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency is only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (tags == null)
                {
                    throw new ArgumentNullException(nameof(tags), "Value cannot be null.");
                }

                if (tags.Count() == 0 || tags.Any(x => x == null))
                {
                    throw new ArgumentException("Either the collection passed is empty or one of the tags passed is null.", nameof(tags));
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheKey = key.Trim();

                var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, tags, _expirable, timeout);

                _cache.Insert(cacheKey, cacheItem);

                return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        internal DataCacheItemVersion Put(string key, object value, TimeSpan timeout, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency and region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency is only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, TimeSpan timeout, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency and region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion version, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency and region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }


        



        internal bool Remove(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                var cacheKey = key.Trim();

                var result = _cache.Remove(cacheKey, out object removedItem);

                return removedItem != null;
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        internal bool Remove(string key, DataCacheItemVersion version)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency is only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal bool Remove(string key, DataCacheLockHandle lockHandle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                var cacheLockHandle = lockHandle.LockHandle;

                var cacheKey = key.Trim();
                var result = _cache.Remove(cacheKey, out object removedItem, cacheLockHandle);

                return removedItem != null;
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        return false;
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal bool Remove(string key, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal bool Remove(string key, DataCacheItemVersion version, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency and region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal bool Remove(string key, DataCacheLockHandle lockHandle, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }







        internal IEnumerable<KeyValuePair<string, object>> GetObjectsByTag(DataCacheTag tag, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal IEnumerable<KeyValuePair<string, object>> GetObjectsByAnyTag(IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        
        internal IEnumerable<KeyValuePair<string, object>> GetObjectsByAllTags(IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        
        internal IEnumerable<KeyValuePair<string, object>> GetObjectsInRegion(string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }






        internal IEnumerable<KeyValuePair<string, object>> BulkGet(IEnumerable<string> keys)
        {
            try
            {
                if (keys == null)
                {
                    throw new ArgumentNullException(nameof(keys), "Value cannot be null.");
                }

                if (keys.Any(x => x == null))
                {
                    throw new ArgumentException("A key is passed with value null.", nameof(keys));
                }

                if (keys.Count() == 0)
                {
                    return new List<KeyValuePair<string, object>>();
                }

                var cacheKeys = keys.Select(x => x.Trim());
                var cachedDict = _cache.GetBulk<object>(cacheKeys);

                var result = new List<KeyValuePair<string, object>>(cachedDict.Count);

                
                foreach (var entry in cachedDict)
                {
                    result.Add(new KeyValuePair<string, object>(entry.Key, entry.Value));
                }

                return result;
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal IEnumerable<KeyValuePair<string, object>> BulkGet(IEnumerable<string> keys, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal DataCacheItem GetCacheItem(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                var cacheKey = key.Trim();

                var cacheItem = _cache.GetCacheItem(cacheKey);

                if (cacheItem == null)
                {
                    return null;
                }

                return cacheItem.GetValue<DataCacheItem>();
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItem GetCacheItem(string key, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }



        internal object GetIfNewer(string key, ref DataCacheItemVersion version)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency is only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal object GetIfNewer(string key, ref DataCacheItemVersion version, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Optimistic concurrency and region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheKey = key.Trim();

                LockHandle cacheLockHandle = null;
                var cacheItem = _cache.GetCacheItem(cacheKey, true, timeout, ref cacheLockHandle);

                if (cacheItem != null)
                {
                    lockHandle = new DataCacheLockHandle(cacheLockHandle);

                    return cacheItem.GetValue<object>();
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Item locked.")
                        {
                            ErrorCode = DataCacheErrorCode.ObjectLocked
                        };
                    }
                    else
                    {
                        throw new DataCacheException("Key not found.")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        internal object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("NCache does not support locking on non-existing objects"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        
        internal object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, string region, bool forceLock)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("NCache does not support locking on non-existing objects"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                var cacheKey = key.Trim();
                var cacheLockHandle = lockHandle.LockHandle;

                var oldCacheItem = _cache.GetCacheItem(cacheKey, false, new TimeSpan(0, 0, 2), ref cacheLockHandle);

                if (oldCacheItem != null)
                {
                    var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, null, _expirable, _defaultTimeout);

                    _cache.Insert(cacheKey, cacheItem, cacheLockHandle, true);

                    return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Invalid lock handle")
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw new DataCacheException("Key does not exist")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }

            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        throw new DataCacheException("Invalid lock handle", ex)
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, IEnumerable<DataCacheTag> tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                if (tags == null)
                {
                    throw new ArgumentNullException(nameof(tags), "Value cannot be null.");
                }

                if (tags.Count() == 0 || tags.Any(x => x == null))
                {
                    throw new ArgumentException("Either the collection passed is empty or one of the tags passed is null.", nameof(tags));
                }

                var cacheKey = key.Trim();
                var cacheLockHandle = lockHandle.LockHandle;

                var oldCacheItem = _cache.GetCacheItem(cacheKey, false, new TimeSpan(0, 0, 2), ref cacheLockHandle);

                if (oldCacheItem != null)
                {
                    var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, tags, _expirable, _defaultTimeout);

                    _cache.Insert(cacheKey, cacheItem, cacheLockHandle, true);

                    return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Invalid lock handle")
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw new DataCacheException("Key does not exist")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }

            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        throw new DataCacheException("Invalid lock handle", ex)
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheKey = key.Trim();
                var cacheLockHandle = lockHandle.LockHandle;

                var oldCacheItem = _cache.GetCacheItem(cacheKey, false, new TimeSpan(0, 0, 2), ref cacheLockHandle);

                if (oldCacheItem != null)
                {
                    var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, null, _expirable, timeout);

                    _cache.Insert(cacheKey, cacheItem, cacheLockHandle, true);

                    return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Invalid lock handle")
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw new DataCacheException("Key does not exist")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }

            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        throw new DataCacheException("Invalid lock handle", ex)
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        
        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                if (tags == null)
                {
                    throw new ArgumentNullException(nameof(tags), "Value cannot be null.");
                }

                if (tags.Count() == 0 || tags.Any(x => x == null))
                {
                    throw new ArgumentException("Either the collection passed is empty or one of the tags passed is null.", nameof(tags));
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheKey = key.Trim();
                var cacheLockHandle = lockHandle.LockHandle;

                var oldCacheItem = _cache.GetCacheItem(cacheKey, false, new TimeSpan(0, 0, 2), ref cacheLockHandle);

                if (oldCacheItem != null)
                {
                    var cacheItem = DataFormatter.CreateCacheItem(key, _cacheName, value, tags, _expirable, timeout);

                    _cache.Insert(cacheKey, cacheItem, cacheLockHandle, true);

                    return new DataCacheItemVersion(cacheItem.GetValue<DataCacheItem>());
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Invalid lock handle")
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw new DataCacheException("Key does not exist")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }

            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        throw new DataCacheException("Invalid lock handle", ex)
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        
        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal void Unlock(string key, DataCacheLockHandle lockHandle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                var cacheKey = key.Trim();

                LockHandle cacheLockHandle = lockHandle.LockHandle;
                var cacheItem = _cache.GetCacheItem(cacheKey, false, new TimeSpan(0, 0, 2), ref cacheLockHandle);

                if (cacheItem != null)
                {
                    _cache.Insert(cacheKey, cacheItem, cacheLockHandle, true);
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Invalid lock handle.")
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw new DataCacheException("Key not found.")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        throw new DataCacheException("Invalid lock handle", ex)
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_WITH_VERSION_DOESNT_EXIST)
                    {
                        throw new DataCacheException("Item updated outside lock", ex)
                        {
                            ErrorCode = DataCacheErrorCode.ObjectNotLocked
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal void Unlock(string key, DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (lockHandle == null)
                {
                    throw new ArgumentNullException(nameof(lockHandle), "Value cannot be null.");
                }

                if (timeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeout));
                }

                var cacheKey = key.Trim();

                LockHandle cacheLockHandle = lockHandle.LockHandle;
                var cacheItem = _cache.GetCacheItem(cacheKey, false, new TimeSpan(0, 0, 2), ref cacheLockHandle);

                if (cacheItem != null)
                {
                    cacheItem.Expiration = new Expiration(ExpirationType.Absolute, timeout);

                    _cache.Insert(cacheKey, cacheItem, cacheLockHandle, true);
                }
                else
                {
                    if (cacheLockHandle.LockId != null)
                    {
                        throw new DataCacheException("Invalid lock handle.")
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }

                    else
                    {
                        throw new DataCacheException("Key not found.")
                        {
                            ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                        };
                    }
                }
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    if (ex.ErrorCode == ErrorCodes.BasicCacheOperations.ITEM_LOCKED)
                    {
                        throw new DataCacheException("Invalid lock handle", ex)
                        {
                            ErrorCode = DataCacheErrorCode.InvalidCacheLockHandle
                        };
                    }
                    else
                    {
                        throw CommonCacheExceptions(ex);
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        internal void Unlock(string key, DataCacheLockHandle lockHandle, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal void Unlock(string key, DataCacheLockHandle lockHandle, TimeSpan timeOut, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal void ResetObjectTimeout(string key, TimeSpan timeOut)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new ArgumentNullException(nameof(key), "Value cannot be null.");
                }

                if (timeOut <= TimeSpan.Zero)
                {
                    throw new ArgumentException("Time-out should be a positive value.", nameof(timeOut));
                }

                var cacheKey = key.Trim();
                var cacheItem = _cache.GetCacheItem(cacheKey);

                if (cacheItem == null)
                {
                    throw new DataCacheException("Key not found.")
                    {
                        ErrorCode = DataCacheErrorCode.KeyDoesNotExist
                    };
                }

                cacheItem.Expiration = new Expiration(ExpirationType.Absolute, timeOut);

                _cache.Insert(cacheKey, cacheItem);
            }
            catch (Exception e)
            {
                CacheException ex = e as CacheException;
                if (ex != null)
                {
                    throw CommonCacheExceptions(ex);
                }
                else
                {
                    throw e;
                }
            }
        }

        internal void ResetObjectTimeout(string key, TimeSpan timeOut, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }





        internal bool CreateRegion(string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal bool RemoveRegion(string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        
        internal void ClearRegion(string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-based operations are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }



        internal string GetSystemRegionName(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(key);
            }

            return "Default_Region";
        }
        internal IEnumerable<string> GetSystemRegions()
        {
            yield return "Default_Region";
        }


        internal DataCacheNotificationDescriptor AddCacheLevelCallback(DataCacheOperations filter, DataCacheNotificationCallback callBack)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Cache-Level Notifications are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheNotificationDescriptor AddRegionLevelCallback(string region, DataCacheOperations filter, DataCacheNotificationCallback callBack)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-Level Notifications are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheNotificationDescriptor AddItemLevelCallback(string key, DataCacheOperations filter, DataCacheNotificationCallback callBack)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Cache-Level Item Notifications are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheNotificationDescriptor AddItemLevelCallback(string key, DataCacheOperations filter, DataCacheNotificationCallback callBack, string region)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Region-Level Item Notifications are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheNotificationDescriptor AddCacheLevelBulkCallback(DataCacheBulkNotificationCallback callBack)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Bulk Cache-Level Notifications are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal DataCacheNotificationDescriptor AddFailureNotificationCallback(DataCacheFailureNotificationCallback callBack)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("NCache does not support failure notifications"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }

        internal void RemoveCallback(DataCacheNotificationDescriptor nd)
        {
            throw new DataCacheException("Operation not supported", new NotSupportedException("Notifications are only supported in NCache Enterprise AppFabric Wrapper"))
            {
                ErrorCode = DataCacheErrorCode.OperationNotSupported
            };
        }


        private static Exception CommonCacheExceptions(CacheException ex)
        {
            if (ex.ErrorCode == ErrorCodes.Common.NO_SERVER_AVAILABLE)
            {
                return new DataCacheException("Server timeout", ex)
                {
                    ErrorCode = DataCacheErrorCode.Timeout
                };

            }
            else if (ex.ErrorCode == ErrorCodes.Common.CONNECTIVITY_LOST)
            {
                return new DataCacheException("Connectivity lost", ex)
                {
                    ErrorCode = DataCacheErrorCode.RetryLater,
                    SubStatus = DataCacheErrorSubStatus.CacheServerUnavailable
                };
            }
            else
            {
                return new DataCacheException("Undefined", ex)
                {
                    ErrorCode = DataCacheErrorCode.UndefinedError
                };
            }
        }
    }
}
