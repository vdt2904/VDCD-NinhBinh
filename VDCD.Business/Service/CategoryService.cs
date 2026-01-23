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
        private readonly IRepository<SeoMeta> _seoRepo;
        public CategoryService(AppDbContext context, IRepository<Category> repo, ICacheService cache, IRepository<SeoMeta> seoRepo) { 
            _cache = cache;
            _categoryService = repo;
            _context = context;
            _seoRepo = seoRepo;
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
        public Category GetById(int Id)
        {
            return _categoryService.GetReadOnly(x=>x.Id == Id);
        }
        public Category Save(Category model,string keywords)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var exists = _categoryService
        .Get(true, x => x.Slug == model.Slug && x.Id != model.Id);

            if (exists != null)
                throw new Exception("Category đã tồn tại (trùng slug)");
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
                entity.Slug = model.Slug;

                _categoryService.Update(entity);
            }

            _context.SaveChanges();
            SaveSeo(entity, keywords);
            // clear cache
            _cache.Remove(CacheParam.CategoryAll);

            return entity;
        }
        public void Delete(int id)
        {
            var entity = _categoryService.Get(id);
            if (entity == null)
                throw new Exception("Category không tồn tại");
            var seoKey = $"category:{entity.Slug}";
            var seo = _seoRepo.Get(false, x => x.Seo_Key == seoKey);
            if (seo != null)
            {
                _seoRepo.Delete(seo);
            }
            _categoryService.Delete(entity);
            _context.SaveChanges();

            _cache.Remove(CacheParam.CategoryAll);
        }
        private void SaveSeo(Category category, string keywords)
        {
            var seoKey = $"category:{category.Slug}";

            var seo = _seoRepo.Get(false, x => x.Seo_Key == seoKey);

            if (seo == null)
            {
                seo = new SeoMeta
                {
                    Seo_Key = seoKey,
                    Title = category.CategoryName,
                    Description = $"Danh mục {category.CategoryName} của công ty",
                    Is_Index = true,
                    Keywords = keywords,
                    created_at = DateTime.Now,
                };

                _seoRepo.Create(seo);
            }
            else
            {
                seo.Title = category.CategoryName;
                seo.Keywords = keywords;
            }

            _context.SaveChanges();
        }

        public Category GetBySlug(string  slug)
        {
            return _categoryService.GetReadOnly(x => x.Slug == slug);
        }
    }
}
