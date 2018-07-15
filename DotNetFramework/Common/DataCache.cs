using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class DataCache
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string cacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[cacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string cacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            int cacheMinutes = ConfigHelper.GetConfigInt("ModelCache");//获取配置文件中的缓存时间
            objCache.Insert(cacheKey, objObject, null, DateTime.Now.AddMinutes(cacheMinutes), TimeSpan.Zero);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string cacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 判定是否存在知道键的缓存信息
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exist(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return !(null == objCache[key]);
        }

        /// <summary>
        /// 删除指定键的缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>缓存信息</returns>
        public static bool Remove(string key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return !(null == objCache.Remove(key));
        }

    }
}
