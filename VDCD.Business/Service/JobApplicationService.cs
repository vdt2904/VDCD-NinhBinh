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
    public class JobApplicationService
    {
        private readonly IRepository<JobApplication> _jobApplicationRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        private readonly IRepository<SeoMeta> _seoRepo;
        private readonly IRealtimeNotifier _notifier;
        private readonly IRepository<JobPosition> _jobPositionRepo;
        public JobApplicationService(IRepository<JobApplication> jobApplicationRepo,
                              ICacheService cache,
                              AppDbContext context, 
                              IRepository<SeoMeta> seoRepo,
                              IRealtimeNotifier notifier,
                              IRepository<JobPosition> jobPositionRepo)
        {
            _jobApplicationRepo = jobApplicationRepo;
            _cache = cache;
            _context = context;
            _seoRepo = seoRepo;
            _notifier = notifier;
            _jobPositionRepo = jobPositionRepo;
        }

        public IEnumerable<JobApplication> GetAll()
        {
            if (_cache.TryGet(CacheParam.JobApplyAll, out List<JobApplication> cached))
                return cached;

            var data = _jobApplicationRepo
                .GetsReadOnly()
                .OrderByDescending(x => x.Id)
                .ToList();

            _cache.Set(
                CacheParam.JobApplyAll,
                data,
                TimeSpan.FromMinutes(CacheParam.JobApplyAllTimeout)
            );

            return data;
        }

        public async Task Save(JobApplication app)
        {
            if(app == null) throw new ArgumentNullException("Không có dữ liệu");
            if(app.Id == 0)
            {
                _jobApplicationRepo.Create(app);
            }
            else
            {
                _jobApplicationRepo.Update(app);
            }
            _context.SaveChanges();
            ClearCache();
            await _notifier.Notify("NewCV", new
            {
                /*                contact.Name,
                                contact.Phone,
                                contact.Email,
                                contact.Title,
                                Time = DateTime.Now.ToString("HH:mm:ss")*/
                name = _jobPositionRepo.Get(true,x=>x.Id == app.JobId).Title,

                time = DateTime.Now.ToString("HH:mm dd/MM")
            });
        }
        private void ClearCache()
        {
            _cache.Remove(CacheParam.JobApplyAll);
        }

        public JobApplication GetById(int id)
        {
            return _jobApplicationRepo.Get(id);
        }

        public void Review(JobApplication app)
        {
            if (app == null) throw new ArgumentNullException("Không có dữ liệu");
            if (app.Id == 0)
            {
                _jobApplicationRepo.Create(app);
            }
            else
            {
                _jobApplicationRepo.Update(app);
            }
            _context.SaveChanges();
            ClearCache();
        }

        public void Delete(int id)
        {
            var entity = _jobApplicationRepo.Get(id);
            if (entity == null)
                throw new Exception("Project không tồn tại");

            // 2️⃣ xóa project
            _jobApplicationRepo.Delete(entity);

            // 3️⃣ commit
            _context.SaveChanges();

            ClearCache();
        }
    }
}
