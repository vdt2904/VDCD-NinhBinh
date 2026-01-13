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
    }
}
