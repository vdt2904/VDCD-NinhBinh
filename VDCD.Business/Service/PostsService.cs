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
    public class PostsService
    {
        private readonly IRepository<Posts> _postsRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        private readonly IRepository<SeoMeta> _seoRepo;
        public PostsService(IRepository<Posts> postsRepo,
                              ICacheService cache,
                              AppDbContext context,
                              IRepository<SeoMeta> seoRepo)
        {
            _postsRepo = postsRepo;
            _cache = cache;
            _context = context;
            _seoRepo = seoRepo;
        }
        public IReadOnlyList<Posts> GetAll()
        {
            if (_cache.TryGet(CacheParam.PostsAll, out List<Posts> cached))
                return cached;

            var data = _postsRepo
                .GetsReadOnly()
                .OrderByDescending(x => x.Id)
                .ToList();

            _cache.Set(
                CacheParam.PostsAll,
                data,
                TimeSpan.FromMinutes(CacheParam.PostsAllTimeout)
            );

            return data;
        }
        public IReadOnlyList<Posts> GetAll(string search = "")
        {
            // Đảm bảo search không bị null để tránh lỗi khi dùng .Contains
            search = search ?? "";

            // 1. Kiểm tra Cache
            if (_cache.TryGet(CacheParam.PostsAll, out List<Posts> cached))
            {
                // Phải .ToList() rồi mới cast sang IReadOnlyList
                return cached.Where(x => x.Title.Contains(search)).ToList();
            }

            // 2. Nếu không có cache, lấy từ Repo
            var data = _postsRepo
                .GetsReadOnly()
                .Where(x => x.Title.Contains(search))
                .OrderByDescending(x => x.Id)
                .ToList();

            // 3. Lưu vào Cache
            _cache.Set(
                CacheParam.PostsAll,
                data,
                TimeSpan.FromMinutes(CacheParam.PostsAllTimeout)
            );

            return data;
        }
        public void Save(Posts model, string keywords)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new Exception("Tiêu bài viết không được để trống");
            var exists = _postsRepo
        .Get(true, x => x.Slug == model.Slug && x.Id != model.Id);

            if (exists != null)
                throw new Exception("bài viết đã tồn tại (trùng slug)");

            if (model.Id == 0)
            {
                // CREATE
                model.PublishedDate = DateTime.UtcNow;
                _postsRepo.Create(model);
            }
            else
            {
                // UPDATE
                var entity = _postsRepo.Get(model.Id);
                if (entity == null)
                    throw new Exception("Project không tồn tại");

                entity.Title = model.Title;
                entity.IsPublished = model.IsPublished;
                entity.Thumbnail = model.Thumbnail;
                entity.CategoryId = model.CategoryId;
                entity.Summary = model.Summary;
                entity.Content = model.Content;
                entity.CategoryId = model.CategoryId;
                entity.PublishedDate = DateTime.Now;

                _postsRepo.Update(entity);
            }

            _context.SaveChanges();
            ClearCache();
            SaveSeo(model, keywords);
        }
        public Posts? GetById(int id)
        {
            return _postsRepo
                .GetsReadOnly()
                .FirstOrDefault(x => x.Id == id);
        }

        // =======================
        // SAVE (CREATE + UPDATE)
        // =======================

        // =======================
        // DELETE
        // =======================
        public void Delete(int id)
        {
            var entity = _postsRepo.Get(id);
            if (entity == null)
                throw new Exception("Bài viết không tồn tại");
            var seoKey = $"project:{entity.Slug}";
            var seo = _seoRepo.Get(false, x => x.Seo_Key == seoKey);
            if (seo != null)
            {
                _seoRepo.Delete(seo);
            }

            // 2️⃣ xóa project
            _postsRepo.Delete(entity);

            // 3️⃣ commit
            _context.SaveChanges();

            ClearCache();
        }

        // =======================
        // CLEAR CACHE
        // =======================
        private void ClearCache()
        {
            _cache.Remove(CacheParam.PostsAll);
        }
        private void SaveSeo(Posts project, string keywords)
        {
            var seoKey = $"post:{project.Slug}";

            var seo = _seoRepo.Get(false, x => x.Seo_Key == seoKey);

            if (seo == null)
            {
                seo = new SeoMeta
                {
                    Seo_Key = seoKey,
                    Title = project.Title,
                    Description = $"Bài viết {project.Title}",
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

        public Posts GetBySlug(string slug)
        {
            return _postsRepo.Get(true,x=>x.Slug == slug);
        }
    }
}
