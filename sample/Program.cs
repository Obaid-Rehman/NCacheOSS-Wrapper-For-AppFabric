using Alachisoft.NCache.Data.Caching;
using System;
using System.Configuration;

namespace NCacheAppFabricConsoleUI
{
    class Program
    {
        static DataCacheFactory myCacheFactory;
        internal static string myObjectForCaching = "This is my Object";
        internal static DataCache myDefaultCache;
        static void Main(string[] args)
        {
            try
            {
                PrepareClient();
                RunSampleTest();
            }
            catch (Exception exp)
            {

                Console.WriteLine(exp.ToString());
            }

            Console.ReadLine();
        }

        private static void RunSampleTest()
        {
            RunAddItemTests();

            RunGetItemTests();

            RunPutItemTests();

            RunRemoveItemTests();

            RunExpirationTests();

            RunPessimisticConcurrencyTests();
        }

        private static void PrepareClient()
        {
            myCacheFactory = new DataCacheFactory();
            var cacheName = ConfigurationManager.AppSettings["CacheId"];
            myDefaultCache = myCacheFactory.GetCache(cacheName);
        }


        private static void RunAddItemTests()
        {
            Logger.WriteHeaderLine("ADD ITEM IN CACHE TESTS");

            AddInCacheTests.AddKeyValuePair();
            AddInCacheTests.AddExistingKeyValuePair();
            AddInCacheTests.AddKeyValuePairWithNullObject();
            AddInCacheTests.AddKeyValuePairWithNullKey();

            Logger.WriteFooterLine("ADD ITEM IN CACHE TESTS");
        }

        private static void RunGetItemTests()
        {
            Logger.WriteHeaderLine("GET ITEM IN CACHE TESTS");

            GetInCacheTests.GetExistingKey();
            GetInCacheTests.GetNonExistingKey();
            GetInCacheTests.GetNullKey();
            GetInCacheTests.GetExistingKeyItemVersion();
            GetInCacheTests.GetNonExistingKeyItemVersion();
            GetInCacheTests.GetExistingKeyWithOutdatedItemVersion();

            Logger.WriteFooterLine("GET ITEM IN CACHE TESTS");
        }

        private static void RunPutItemTests()
        {
            Logger.WriteHeaderLine("PUT ITEM IN CACHE TESTS");

            PutInCacheTests.PutExistingKey();
            PutInCacheTests.PutNonExistingKey();
            PutInCacheTests.PutKeyWithNullValue();

            Logger.WriteFooterLine("PUT ITEM IN CACHE TESTS");
        }

        private static void RunRemoveItemTests()
        {
            Logger.WriteHeaderLine("REMOVE ITEM IN CACHE TESTS");

            RemoveInCacheTests.RemoveExistingKey();
            RemoveInCacheTests.RemoveNonExistingKey();
            RemoveInCacheTests.RemoveNullKey();

            Logger.WriteFooterLine("REMOVE ITEM IN CACHE TESTS");
        }

        private static void RunExpirationTests()
        {
            Logger.WriteHeaderLine("EXPIRATION TESTS");

            ExpirationTests.AddKeyValuePairWithNegativeTimeSpan();
            ExpirationTests.AddKeyValuePairWithZeroTimeSpan();
            ExpirationTests.PutKeyValuePairWithNegativeTimeSpan();
            ExpirationTests.PutKeyValuePairWithZeroTimeSpan();

            Logger.WriteFooterLine("EXPIRATION TESTS");
        }

        private static void RunPessimisticConcurrencyTests()
        {
            Logger.WriteHeaderLine("PESSIMISTIC CONCURRENCY TESTS");

            PessimisticConcurrencyTests
                .GetLockOnExistingKey();

            PessimisticConcurrencyTests
                .GetLockOnNonExistingKey();

            PessimisticConcurrencyTests
                .GetLockOnLockedKey();


            PessimisticConcurrencyTests
                .GetLockWithNegativeLockTimeout();

            PessimisticConcurrencyTests
                .GetLockWithZeroLockTimeout();

            PessimisticConcurrencyTests
                .PutAndUnlockWithValidLockHandle();

            PessimisticConcurrencyTests
                .PutAndUnlockWithValidLockHandleNegativeExpirationTimeout();

            PessimisticConcurrencyTests
                .PutAndUnlockWithValidLockHandleZeroLockTimeout();

            PessimisticConcurrencyTests
                .PutAndUnlockWithInvalidLockHandle();

            PessimisticConcurrencyTests
                .PutAndUnlockWithNullLockHandle();

            PessimisticConcurrencyTests
                .UnlockWithValidLockHandle();

            PessimisticConcurrencyTests
                .UnlockWithValidLockHandleNegativeExpirationTimeout();

            PessimisticConcurrencyTests
                .UnlockWithValidLockHandleZeroLockTimeout();

            PessimisticConcurrencyTests
                .UnlockWithInvalidLockHandle();

            PessimisticConcurrencyTests
                .UnlockWithNullLockHandle();

            PessimisticConcurrencyTests
                .RemoveWithValidLockHandle();

            PessimisticConcurrencyTests
                .RemoveWithInvalidLockHandle();

            PessimisticConcurrencyTests
                .RemoveWithNullLockHandle();


            Logger.WriteFooterLine("PESSIMISTIC CONCURRENCY TESTS");
        }

    }
}
