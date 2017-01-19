using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alachisoft.NCache.Web.Caching;
using Alachisoft.NCache.Web;
using Alachisoft.NCache.Runtime.Caching;
using System.Collections;
using System.Configuration;
using System.Runtime.Serialization;
using Alachisoft.NCache.Data.Caching.Util;
using System.Threading;

namespace Alachisoft.NCache.Data.Caching.Handler
{
    /// <summary>
    /// provides handler for all internal NCache calls
    /// </summary>
    internal class CacheHandler
    {
        #region [private members]
        
        private DataFormatter _formatter;
        private Cache _NCache;
        private bool _addRegionCheck;
        private bool _removeRegionCheck;
        private bool _clearRegionCheck;

        private TimeSpan _defaultLockTime;
        private int _retries;
        private TimeSpan _retryTimeOut;
        #endregion

        #region[    Constructor    ]
        internal CacheHandler(Alachisoft.NCache.Web.Caching.Cache cache, TimeSpan defaultLockTime, int retries, TimeSpan retryTimeOut)
        {
           
            _NCache = cache;
            _addRegionCheck=false;
            _removeRegionCheck = false;
            _clearRegionCheck = false;
            _defaultLockTime = defaultLockTime;
            _retries = retries;
            _retryTimeOut = retryTimeOut;
            _formatter = new DataFormatter();
           
        }
        #endregion


        internal DataCacheItemVersion Add(string key, object value, TimeSpan timeOut, string region)
        {
            string expirationCheck = ConfigurationManager.AppSettings["Expirable"];
            
            try
            {
                CacheItem _item=null;
                if (expirationCheck.Equals("True"))
                {
                    if (timeOut == TimeSpan.Zero)
                    {
                        string TTL = ConfigurationManager.AppSettings["TTL"];
                        TimeSpan tempTimeOut = TimeSpan.Parse(TTL);
                        if (!String.IsNullOrWhiteSpace(TTL) && tempTimeOut != TimeSpan.Zero)
                        {
                            _item = _formatter.CreateCacheItem(value, region, tempTimeOut);
                        }
                        else if (String.IsNullOrWhiteSpace(TTL))
                        {
                            _item = _formatter.CreateCacheItem(value, region, TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        _item = _formatter.CreateCacheItem(value, region,timeOut);
                    }
                }
                else if (expirationCheck.Equals("False"))
                {
                    _item = _formatter.CreateCacheItem(value, region);
                }
                string _key = _formatter.MarshalKey(key,region);

                _NCache.Add(_key, _item);
                return _formatter.ConvertToAPVersion(1); // Always the first Item
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        internal void Clear()
        {
            try
            {
                _NCache.Clear();
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        internal void CreateRegion(string region)
        {
            //if (_addRegionCheck == true)
            //{
            //    _NCache.RaiseCustomEvent(region, CallbackType.AddRegion);
            //}
        }

        [Obsolete("Region callbacks are only supported in NCache Enterprise", true)]
        internal void RegisterRegionCallBack(CallbackType opcode)
        {
            throw new NotSupportedException("Region Callbacks are only supported in NCache Enterprise");
        }

        [Obsolete("Region callbacks are only supported in NCache Enterprise", true)]
        internal void UnRegisterRegionCallBack(CallbackType opcode)
        {
            throw new NotSupportedException("Region Callbacks are only supported in NCache Enterprise");
        }

        internal object Get(string key, out DataCacheItemVersion version, string region)
        {
            string _key;
            object obj;
            version = (DataCacheItemVersion)FormatterServices.GetUninitializedObject(typeof(DataCacheItemVersion));
            if (string.IsNullOrWhiteSpace(region))
            {
                _key = _formatter.MarshalKey(key, null);
            }
            else
            {
                _key = _formatter.MarshalKey(key, region);
            }
            try
            {

                MetaDataCapsule metadata = _NCache.Get(_key) as MetaDataCapsule;
                if (metadata == null)
                    return null;

                obj = metadata.Value;


                if (version._itemVersion != null && version._itemVersion != metadata.CacheItemVersion)
                {
                    return null;
                }
                else
                {
                    version._itemVersion = metadata.CacheItemVersion;
                }

                if (obj != null)
                {
                    return obj;
                }
                else
                    return null;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        internal IEnumerable<KeyValuePair<string, object>> GetBulk(IEnumerable<string> keys, string region)
        {
            string[] keyList = _formatter.MarshalKey(keys, region);
            
            IDictionary _temp = _NCache.GetBulk(keyList);
            IDictionaryEnumerator _tempEnumerator = (IDictionaryEnumerator)_temp.GetEnumerator();
            while (_tempEnumerator.MoveNext())
            {
                DictionaryEntry item = _tempEnumerator.Entry;
                yield return new KeyValuePair<string, object>(_formatter.UnMarshalKey(item.Key.ToString()), (item.Value as MetaDataCapsule).Value);
            }
        }

        [Obsolete("Tags are only supported in NCache Enterprise", true)]
        internal IEnumerable<KeyValuePair<string, object>> GetObjectsByTag(string region, DataCacheTag tag)
        {
            throw new NotSupportedException("Tags are only supported in NCache Enterprise");
        }
        
        [Obsolete("Tags are only supported in NCache Enterprise", true)]
        internal IEnumerable<KeyValuePair<string, object>> 
            GetObjectsByAnyTag(IEnumerable<DataCacheTag> tags, string region)
        {
            throw new NotSupportedException("Tags are only supported in NCache Enterprise");
        }

        [Obsolete("Tags are only supported in NCache Enterprise", true)]
        internal IEnumerable<KeyValuePair<string, object>> GetObjectsByAllTags(IEnumerable<DataCacheTag> tags, string region)
        {
            throw new NotSupportedException("Tags are only supported in NCache Enterprise");
        }

        [Obsolete("Regions are kept as Groups in NCache and Groups are only supported in NCache Enterprise", true)]
        internal IEnumerable<KeyValuePair<string, object>> GetObjectsInRegion(string region) 
        {
            throw new NotSupportedException("Regions are kept as Groups in NCache and Groups are only supported in NCache Enterprise");
        }

        internal object GetLock(string key, TimeSpan timeOut, out DataCacheLockHandle appLockHandle, string region, bool forceLock)
        {
            appLockHandle = new DataCacheLockHandle();
            object obj;
            LockHandle lockHandle = _formatter.ConvertToNCacheLockHandle(appLockHandle);
            try
            {
                if (string.IsNullOrWhiteSpace(region))
                {
                    key = _formatter.MarshalKey(key, null);
                }
                else
                {
                    key = _formatter.MarshalKey(key, region);
                }
                object cacheGetAndLockResult = _NCache.Get(key, timeOut, ref lockHandle, true);

                if (cacheGetAndLockResult == null)
                {
                    obj = _NCache.Get(key, timeOut, ref lockHandle, true);
                }
                else
                {
                    obj = cacheGetAndLockResult;
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Tries to acqurie lock and return LockHandle and MetaDataCapsule object. If LockHandle is provided uses that instead.
        /// <para>Default Lock Timeout (default 5 seconds), Retry Count (default 3) 
        /// and retry interval (default 100 ms) is set when Cache Handler is instantiated.</para>
        /// </summary>
        /// <param name="key">Formatted Key i.e compile from formatter</param>
        /// <param name="lockHandle">Either provide a lock or keep it null to acquire a new lock</param>
        /// <param name="metadata">The returned object for internal use</param>
        /// <returns>true if lock was acquired</returns>
        internal bool GetLock(string key, ref LockHandle lockHandle, out MetaDataCapsule metadata)
        {
            //Item is locked
            int count = _retries;
            while (count != 0)
            {
                //If lock was provided attempt to acquire the lock from the given handle
                if (lockHandle == null)
                {
                    lockHandle = new LockHandle(); 
                }

                //1. If item does not exist: obj is null & lockHandle will have lockId empty.
                //2. if Item exists but but already locked; obj is null and lockHandle will be updated with the acquired lock
                //3. If Item exists but free for locking; obj is returned with object and lock will be updated with the correct lockId.
                object obj = _NCache.Get(key, _defaultLockTime, ref lockHandle, true);

                //obj is null if the lock was not acquried
                if (obj == null)
                {
                    //Item does not exist
                    if (String.IsNullOrWhiteSpace(lockHandle.LockId))
                    {
                        metadata = null;
                        return false;
                    }
                    count--;
                    Thread.Sleep(_retryTimeOut);
                }
                else
                {
                    metadata = obj as MetaDataCapsule;
                    return true;
                }
            }

            lockHandle = null;
            metadata = null;
            return false;

        }
                
        internal DataCacheItem GetCacheItem(string key, string region,string CacheName)
        {
            try
            {
                string _key;
                DataCacheItem _dataCacheItem;
                if (string.IsNullOrWhiteSpace(region))
                {
                    _key = _formatter.MarshalKey(key, null);
                }
                else
                {
                    _key = _formatter.MarshalKey(key, region);
                }
                _dataCacheItem = _formatter.ConvertToDataCacheItem(_NCache.GetCacheItem(_key));
                string[] items = _formatter.SplitKeyAndRegion(_key);
                _dataCacheItem.Key = items[0];
                _dataCacheItem.CacheName = CacheName;
                _dataCacheItem.RegionName = items[1];

                return _dataCacheItem;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        
        internal object GetIfNewer(string key, ref DataCacheItemVersion version, string region)
        {
            
            string _key = _formatter.MarshalKey(key,region);

            MetaDataCapsule metadata = _NCache.Get(_key) as MetaDataCapsule;
            if (version._itemVersion < metadata.CacheItemVersion)
            {
                return metadata.Value;
            }
            else
            {
                return null;
            }
       
        }
       
        internal string GetSystemRegionName(string key)
        {
            string _key = _formatter.MarshalKey(key);
            MetaDataCapsule metadata = _NCache.Get(_key) as MetaDataCapsule;
            if (metadata != null)
            {
                return metadata.Group;
            }
            else
            {
                return null;
            }
        }
        
        internal DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeOut,
            IEnumerable<DataCacheTag> tags, string region)
        {
            CacheItem _item=_formatter.CreateCacheItem(value, region, timeOut);
            MetaDataCapsule metadata = _item.Value as MetaDataCapsule;

            string _key;

            if (string.IsNullOrWhiteSpace(region))
            {
                _key = _formatter.MarshalKey(key, null);
            }
            else
            {
                _key = _formatter.MarshalKey(key, region);
            }

            LockHandle lockhandle = null;
            MetaDataCapsule cachedMetadata;

            bool lockAcquired = GetLock(_key, ref lockhandle, out cachedMetadata);

            if(!lockAcquired)
            {
                //Item does not exist
                if (lockhandle != null && String.IsNullOrWhiteSpace(lockhandle.LockId))
                    lockhandle = new LockHandle();
                else
                    throw new DataCacheException("Unable to acqurie lock to update Item Version");
            }

            if (oldVersion != null)
            {
                //Return in case of wrong version provided
                if (cachedMetadata == null)
                {
                    //Item did not exist
                    return null;
                }
                else if (cachedMetadata.CacheItemVersion == oldVersion._itemVersion)
                {
                    _NCache.Unlock(key);
                    return null;
                }
            }

            if (lockAcquired)
            {
                metadata.CacheItemVersion = ++cachedMetadata.CacheItemVersion; 
            }
            else
            {
                metadata = MetaDataCapsule.Encapsulate(value, region);
            }

            _NCache.Insert(_key, _item, lockhandle, true);

            return _formatter.ConvertToAPVersion(metadata.CacheItemVersion);
        }

        internal DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeOut,
            IEnumerable<DataCacheTag> tags, string region)
        {
            CacheItem _item = _formatter.CreateCacheItem(value, region, timeOut);
            string _key;
            if (string.IsNullOrWhiteSpace(region))
            {
                _key = _formatter.MarshalKey(key);
            }
            else
                _key = _formatter.MarshalKey(key, region);

            LockHandle nLockHandle = _formatter.ConvertToNCacheLockHandle(lockHandle);
            MetaDataCapsule metadata;

            bool lockAcquired = GetLock(_key, ref nLockHandle, out metadata);

            if (!lockAcquired)
            {
                _NCache.Unlock(key);
                throw new DataCacheException("Unable to acqurie lock to update Item Version"); 
            }

            long newversion = metadata.CacheItemVersion + 1;

            if (lockAcquired)
            {
                (_item.Value as MetaDataCapsule).CacheItemVersion = newversion;
            }

            _NCache.Insert(_key, _item, nLockHandle, true);

            return _formatter.ConvertToAPVersion(newversion);
        }

        internal bool Remove(string key, DataCacheLockHandle lockHandle, string region, DataCacheItemVersion version,RemoveOperation opCode)
        {
            if (String.IsNullOrWhiteSpace(region))
            {
                key = _formatter.MarshalKey(key);
            }
            else
            {
                key = _formatter.MarshalKey(key, region);
            }
            
            if (opCode== RemoveOperation.LockBased)
            {
                LockHandle nLockHandle = _formatter.ConvertToNCacheLockHandle(lockHandle);
                if (_NCache.Remove(key, nLockHandle) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(opCode== RemoveOperation.VersionBased) 
            {
                ItemVersion itemVersion = _formatter.ConvertToNCacheVersion(version);
                LockHandle lockhandle = null;
                MetaDataCapsule meta;
                bool lockAcquired = GetLock(key, ref lockhandle, out meta);
                if (lockAcquired && meta.CacheItemVersion == itemVersion)
                {
                    if (_NCache.Remove(key, lockhandle) != null)
                    {
                        return true;
                    }
                    else
                    {
                        _NCache.Unlock(key, lockhandle);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                
            }
            else if (opCode == RemoveOperation.KeyBased)
            {
                if (_NCache.Remove(key) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [Obsolete("Regions are stored as Groups in NCache, a feature avaiable only in NCache Enterprise", true)]
        internal bool RemoveRegionData(string region,CallbackType opCode)
        {
            throw new NotSupportedException("Regions are stored as Groups in NCache, a feature avaiable only in NCache Enterprise");
        }

        internal bool ResetObjectTimeout(string key, TimeSpan newTimeOut, string region)
        {
            string _key;

            if (string.IsNullOrWhiteSpace(region))
            {
                _key = _formatter.MarshalKey(key, null);
            }
            else
            {
                _key = _formatter.MarshalKey(key, region);
            }
            CacheItem _item = (CacheItem)_NCache.Get(_key);
            if (newTimeOut != TimeSpan.Zero)
            {
                _item.AbsoluteExpiration = DateTime.Now.Add(newTimeOut); 
            }
            else
            {
                _item.AbsoluteExpiration = System.DateTime.Now.AddMinutes(10.0);
            }

            try
            {
                _NCache.Insert(_key, _item);
            }
            catch (Alachisoft.NCache.Runtime.Exceptions.OperationFailedException opfailed)
            {
                return false;
            }
            return true;
            
        }

        internal bool Unlock(string key, DataCacheLockHandle appLockHandle, TimeSpan timeOut, string region)
        {
            if(appLockHandle == null || appLockHandle._lockHandle == null)
            {
                return false;
            }

            LockHandle lockHandle = _formatter.ConvertToNCacheLockHandle(appLockHandle);

            string _key;

            if (string.IsNullOrWhiteSpace(region))
            {
                _key = _formatter.MarshalKey(key, null);
            }
            else
            {
                _key = _formatter.MarshalKey(key, region);
            }

            if (timeOut != TimeSpan.Zero)
            {
                CacheItem _item = (CacheItem)_NCache.GetCacheItem(_key);
                _item.AbsoluteExpiration = DateTime.Now.Add(timeOut);

                try
                {
                    _NCache.Insert(_key, _item, lockHandle, true);
                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                _NCache.Unlock(_key, lockHandle);
            }
            return true;
        }
           
    }
}
