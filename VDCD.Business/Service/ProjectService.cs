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
        public ProjectService(IRepository<Project> projectRepo,
                              ICacheService cache,
                              AppDbContext context)
        {
            _projectRepo = projectRepo;
            _cache = cache;
            _context = context;
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
        public void Save(Project model)
        {
            if (string.IsNullOrWhiteSpace(model.ProjectName))
                throw new Exception("Tên project không được để trống");

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

                _projectRepo.Update(entity);
            }

            _context.SaveChanges();
            ClearCache();
        }

        // =======================
        // DELETE
        // =======================
        public void Delete(int id)
        {
            var entity = _projectRepo.Get(id);
            if (entity == null)
                throw new Exception("Project không tồn tại");

            _projectRepo.Delete(entity);
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
    }
}
