using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class SettingService
    {
        private readonly IRepository<Setting> _settingService;
        private readonly ICacheService _cache;

        public SettingService(IRepository<Setting> repo, ICacheService cache)
        {
            _settingService = repo;
            _cache = cache;
        }

        public string Get(string key)
        {
            var cacheKey = string.Format(CacheParam.SettingByKey, key);

            if (_cache.TryGet(cacheKey, out string value))
                return value;

            var setting = _settingService.GetReadOnly(x => x.SettingKey == key);
            if (setting == null) return null;

            _cache.Set(
                cacheKey,
                setting.Value,
                TimeSpan.FromMinutes(CacheParam.SettingByKeyTimeout)
            );

            return setting.Value;
        }
        public IReadOnlyList<Setting> GetAll()
        {
            if (_cache.TryGet(CacheParam.SettingAllKey, out List<Setting> cached))
                return cached;

            var data = _settingService
                .GetsReadOnly()
                .ToList();

            _cache.Set(
                CacheParam.SettingAllKey,
                data,
                TimeSpan.FromMinutes(CacheParam.SettingAllTimeout)
            );

            return data;
        }
        public void Save(string key, string value)
        {
            var entity = _settingService.Get(false, x => x.SettingKey == key);

            if (entity == null)
                _settingService.Create(new Setting { SettingKey = key, Value = value });
            else
                entity.Value = value;

            // clear cache
            _cache.Remove(string.Format(CacheParam.SettingByKey, key));
            _cache.Remove(CacheParam.SettingAllKey);
        }
    }

}
