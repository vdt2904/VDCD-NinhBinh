using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class SeoMetaService
    {
        private readonly IRepository<SeoMeta> _seoService;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public SeoMetaService(AppDbContext context, IRepository<SeoMeta> repo, ICacheService cache)
        {
            _seoService = repo;
            _cache = cache;
            _context = context;
        }
        public SeoMeta Get(string key)
        {
            return _seoService.Get(true,x=>x.Seo_Key == key);
        }
        public IEnumerable<SeoMeta> Gets()
        {
            return _seoService.Gets(true);
        }
        public void Save(SeoMeta model)
        {
            if (string.IsNullOrWhiteSpace(model.Seo_Key))
                throw new Exception("Tên project không được để trống");
            var entity = _seoService.Get(false,x=>x.Seo_Key == model.Seo_Key);
            if (model.Id == 0 && entity == null)
            {
                // CREATE
                _seoService.Create(model);
            }else if(model.Id == 0 && entity != null)
            {
                throw new Exception("Tên seo_key đã tồn tại");
            }
            else
            {
                // UPDATE
                
                if (entity == null)
                    throw new Exception("Project không tồn tại");

                entity.updated_at = DateTime.Now;
                entity.Description = model.Description;
                entity.Keywords = model.Keywords;
                entity.Title = model.Title;
                entity.Is_Index = model.Is_Index;

                _seoService.Update(entity);
            }

            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var entity = _seoService.Get(id);
            if (entity == null)
                throw new Exception("Seo không tồn tại");

            _seoService.Delete(entity);
            _context.SaveChanges();
        }

        public IEnumerable<SeoMeta> GetAll()
        {
            return _seoService.GetsReadOnly().ToList();
        }
    }
}
