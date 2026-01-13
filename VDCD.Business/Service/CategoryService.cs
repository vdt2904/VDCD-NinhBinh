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
    public class CategoryService
    {
        private readonly IRepository<Category> _categoryService;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public CategoryService(AppDbContext context, IRepository<Category> repo, ICacheService cache) { 
            _cache = cache;
            _categoryService = repo;
            _context = context;
        }
        public IReadOnlyList<Category> GetAll()
        {
            if (_cache.TryGet(CacheParam.CategoryAll, out List<Category> cached))
                return cached;

            var data = _categoryService
                .GetsReadOnly()
                .ToList();

            _cache.Set(
                CacheParam.CategoryAll,
                data,
                TimeSpan.FromMinutes(CacheParam.CategoryAllTimeout)
            );

            return data;
        }
        public Category Save(Category model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Category entity;

            // CREATE
            if (model.Id == 0)
            {
                entity = new Category
                {
                    CategoryName = model.CategoryName,
                    Description = model.Description
                };

                _categoryService.Create(entity);
            }
            // UPDATE
            else
            {
                entity = _categoryService.Get(model.Id);
                if (entity == null)
                    throw new Exception("Category không tồn tại");

                entity.CategoryName = model.CategoryName;
                entity.Description = model.Description;

                _categoryService.Update(entity);
            }

            _context.SaveChanges();

            // clear cache
            _cache.Remove(CacheParam.CategoryAll);

            return entity;
        }
        public void Delete(int id)
        {
            var entity = _categoryService.Get(id);
            if (entity == null)
                throw new Exception("Category không tồn tại");

            _categoryService.Delete(entity);
            _context.SaveChanges();

            _cache.Remove(CacheParam.CategoryAll);
        }


    }
}
