using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VDCD.Business.Service
{
    public class CenterService
    {
        private readonly IRepository<Center> _centerService;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public CenterService(AppDbContext context, IRepository<Center> repo, ICacheService cache)
        {
            _cache = cache;
            _centerService = repo;
            _context = context;
        }
        public IReadOnlyList<Center> GetAll()
        {
            if (_cache.TryGet(CacheParam.CenterAll, out List<Center> cached))
                return cached;
            // Bạn có thể thêm CacheParam.CenterAll vào class CacheParam của mình
            var data = _centerService.GetsReadOnly().OrderByDescending(x => x.Id).ToList();
            _cache.Set(
                CacheParam.ProjectAll,
                data,
                TimeSpan.FromMinutes(CacheParam.CenterAllTimeout)
            );
            return data;
        }

        public void Save(Center model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new Exception("Tên center không được để trống");

            if (model.Id == 0)
            {
                _centerService.Create(model);
            }
            else
            {
                var entity = _centerService.Get(model.Id);
                if (entity == null) throw new Exception("Không tồn tại");

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Url = model.Url;
                entity.Image = model.Image;
                entity.Icon = model.Icon;

                _centerService.Update(entity);
            }
            _context.SaveChanges();
            ClearCache();
        }

        public void Delete(int id)
        {
            var entity = _centerService.Get(id);
            if (entity != null)
            {
                _centerService.Delete(entity);
                _context.SaveChanges();
                ClearCache();
            }
        }
        private void ClearCache()
        {
            _cache.Remove(CacheParam.CenterAll);
        }
    }
}
