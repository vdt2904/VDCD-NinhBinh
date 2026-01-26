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
    public class JobPositionService
    {
        private readonly IRepository<JobPosition> _jobPositionRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        private readonly IRepository<SeoMeta> _seoRepo;
        public JobPositionService(IRepository<JobPosition> jobPositionRepo,
                              ICacheService cache,
                              AppDbContext context,
                              IRepository<SeoMeta> seoRepo)
        {
            _jobPositionRepo = jobPositionRepo;
            _cache = cache;
            _context = context;
            _seoRepo = seoRepo;
        }

        public IEnumerable<JobPosition> GetAll()
        {
            if (_cache.TryGet(CacheParam.JobPositionAll, out List<JobPosition> cached))
                return cached;

            var data = _jobPositionRepo
                .GetsReadOnly()
                .OrderByDescending(x => x.ExpiredDate)
                .ToList();

            _cache.Set(
                CacheParam.JobPositionAll,
                data,
                TimeSpan.FromMinutes(CacheParam.JobPositionAllTimeout)
            );

            return data;
        }

        public void Save(JobPosition model, string keywords)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new Exception("Vị trí tuyển dụng không được để trống");
            var exists = _jobPositionRepo
        .Get(true, x => x.Slug == model.Slug && x.Id != model.Id);

            if (exists != null)
                throw new Exception("Vị trí tuyển dụng đã tồn tại (trùng slug)");

            if (model.Id == 0)
            {
                // CREATE
                model.CreateDate = DateTime.Now;
                _jobPositionRepo.Create(model);
            }
            else
            {
                // UPDATE
                var entity = _jobPositionRepo.Get(model.Id);
                if (entity == null)
                    throw new Exception("Project không tồn tại");

                entity.Title = model.Title;
                entity.Description = model.Description;
                entity.EmploymentType = model.EmploymentType;
                entity.IsActive = model.IsActive;
                entity.Location = model.Location;
                entity.ExpiredDate = model.ExpiredDate;
                entity.Slug = model.Slug;
                entity.Salary = model.Salary;

                _jobPositionRepo.Update(entity);
            }

            _context.SaveChanges();
            ClearCache();
            SaveSeo(model, keywords);
        }
        private void ClearCache()
        {
            _cache.Remove(CacheParam.JobPositionAll);
        }
        private void SaveSeo(JobPosition project, string keywords)
        {
            var seoKey = $"jobposition:{project.Slug}";

            var seo = _seoRepo.Get(false, x => x.Seo_Key == seoKey);

            if (seo == null)
            {
                seo = new SeoMeta
                {
                    Seo_Key = seoKey,
                    Title = project.Title,
                    Description = $"Tuyển dụng vị trí {project.Title} cho công ty",
                    Is_Index = true,
                    Keywords = keywords,
                    created_at = DateTime.Now,
                };

                _seoRepo.Create(seo);
            }
            else
            {
                seo.Title = project.Title;
                seo.Keywords = keywords;
            }

            _context.SaveChanges();
        }

        public JobPosition GetById(int id)
        {
            return _jobPositionRepo.Get(true, x => x.Id == id); 
        }

        public void Delete(int id)
        {
            var delete = _jobPositionRepo.Get(false,x=>x.Id == id);
            if(delete == null)
            {
                throw new Exception("Không tìm thấy JD");
            }
            var seoKey = $"jobposition:{delete.Slug}";
            var seo = _seoRepo.Get(false, x => x.Seo_Key == seoKey);
            if (seo != null)
            {
                _seoRepo.Delete(seo);
            }
            _jobPositionRepo.Delete(delete);
            _context.SaveChanges();
            ClearCache();
        }

        public JobPosition GetBySlug(string slug)
        {
            return _jobPositionRepo.GetReadOnly(x => x.Slug == slug);
        }
    }
}
