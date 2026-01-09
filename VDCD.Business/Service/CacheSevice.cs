using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;

namespace VDCD.Business.Service
{
    public class CacheSevice : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheSevice(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGet<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
        public void ClearAll()
        {
            if (_cache is MemoryCache memCache)
            {
                // MemoryCache không có Clear, nhưng có cách reset bằng cách Dispose + recreate
                memCache.Compact(1.0); // Xóa 100% cache
            }
        }
    }

}
