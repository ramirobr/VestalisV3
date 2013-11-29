using System;
using System.Web;

namespace Cotecna.Vestalis.Core
{
    public static class CacheHandler 
    {
        /// <summary>
        /// Insert or get cache data
        /// </summary>
        /// <typeparam name="T">Type stored in the cache</typeparam>
        /// <param name="cacheId">Cache key</param>
        /// <param name="getItemCallback">Method that return the results to be inserted in the cache</param>
        /// <returns>Value stored in the cache</returns>
        public static T Get<T>(string cacheId, Func<T> getItemCallback) where T : class
        {
            //Try to get the data from the cache
            T item = HttpRuntime.Cache.Get(cacheId) as T;
            if (item == null)
            {
                //Execute the method to get the data to be inserted in the cache
                item = getItemCallback();
                //Insert into cache. Expiration 1 day
                HttpRuntime.Cache.Add(cacheId, item, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default, null);
            }
            return item;
        }

        public static void Remove(string cacheId)
        {
            HttpRuntime.Cache.Remove(cacheId);
        }
    }
}
