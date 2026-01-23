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
    public class ProjectService
    {
        private readonly IRepository<Project> _projectRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        private readonly IRepository<SeoMeta> _seoRepo;
        public ProjectService(IRepository<Project> projectRepo,
                              ICacheService cache,
                              AppDbContext context,
                              IRepository<SeoMeta> seoRepo)
        {
            _projectRepo = projectRepo;
            _cache = cache;
            _context = context;
            _seoRepo = seoRepo;
        }

        // =======================
        // GET ALL
        // =======================
        public IReadOnlyList<Project> GetAll()
        {
            if (_cache.TryGet(CacheParam.ProjectAll, out List<Project> cached))
                return cached;

            var data = _projectRepo
                .GetsReadOnly()
                .OrderByDescending(x => x.Id)
                .ToList();

            _cache.Set(
                CacheParam.ProjectAll,
                data,
                TimeSpan.FromMinutes(CacheParam.ProjectAllTimeout)
            );

            return data;
        }

        // =======================
        // GET BY ID
        // =======================
        public Project? GetById(int id)
        {
            return _projectRepo
                .GetsReadOnly()
                .FirstOrDefault(x => x.Id == id);
        }

        // =======================
        // SAVE (CREATE + UPDATE)
        // =======================
        public void Save(Project model,string keywords)
        {
            if (string.IsNullOrWhiteSpace(model.ProjectName))
                throw new Exception("Tên project không được để trống");
            var exists = _projectRepo
        .Get(true,x => x.Plug == model.Plug && x.Id != model.Id);

            if (exists != null)
                throw new Exception("Project đã tồn tại (trùng slug)");

            if (model.Id == 0)
            {
                // CREATE
                _projectRepo.Create(model);
            }
            else
            {
                // UPDATE
                var entity = _projectRepo.Get(model.Id);
                if (entity == null)
                    throw new Exception("Project không tồn tại");

                entity.ProjectName = model.ProjectName;
                entity.Description = model.Description;
                entity.Json = model.Json;
                entity.CategoryId = model.CategoryId;
                entity.Investor = model.Investor;
                entity.IsActive = model.IsActive;
                entity.Location = model.Location;
                entity.Image = model.Image;
                entity.Time = model.Time;
                entity.Plug = model.Plug;

                _projectRepo.Update(entity);
            }

            _context.SaveChanges();
            ClearCache();
            SaveSeo(model, keywords);
        }

        // =======================
        // DELETE
        // =======================
        public void Delete(int id)
        {
            var entity = _projectRepo.Get(id);
            if (entity == null)
                throw new Exception("Project không tồn tại");
            var seoKey = $"project:{entity.Plug}";
            var seo = _seoRepo.Get(false,x => x.Seo_Key == seoKey);
            if (seo != null)
            {
                _seoRepo.Delete(seo);
            }

            // 2️⃣ xóa project
            _projectRepo.Delete(entity);

            // 3️⃣ commit
            _context.SaveChanges();

            ClearCache();
        }

        // =======================
        // CLEAR CACHE
        // =======================
        private void ClearCache()
        {
            _cache.Remove(CacheParam.ProjectAll);
        }
        private void SaveSeo(Project project,string keywords)
        {
            var seoKey = $"project:{project.Plug}";

            var seo = _seoRepo.Get(false,x => x.Seo_Key == seoKey);

            if (seo == null)
            {
                seo = new SeoMeta
                {
                    Seo_Key = seoKey,
                    Title = project.ProjectName,
                    Description = $"Dự án {project.ProjectName} của công ty",
                    Is_Index = true,
                    Keywords = keywords,
                    created_at = DateTime.Now,
                };

                _seoRepo.Create(seo);
            }
            else
            {
                seo.Title = project.ProjectName;
                seo.Keywords = keywords;
            }

            _context.SaveChanges();
        }

        public Project GetBySlug(string slug)
        {
            return _projectRepo.GetReadOnly(x=>x.Plug == slug);
        }
    }
}
