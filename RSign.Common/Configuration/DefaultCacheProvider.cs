using RSign.Common.Configuration.Interfaces;
using System;
using System.Linq;
using System.Runtime.Caching;

namespace RSign.Common.Configuration
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        public object Get(string key)
        {
            return Cache[key];
        }

        public void Set(string key, object data, int cacheTime)
        {
            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime) };
            Cache.Add(new CacheItem(key, data), policy);
        }

        public bool IsSet(string key)
        {
            return Cache[key] != null;
        }

        public void Invalidate(string key)
        {
            Cache.Remove(key);
        }

        public void Refresh(string key)
        {
            // Cache.
        }

        public void ClearAll()
        {
            while (Cache.Any())
            {
                Cache.Remove(Cache.First().Key);
            }
        }
    }
}
