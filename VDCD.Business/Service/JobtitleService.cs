using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.DataAccess;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class JobtitleService
    {
        private readonly IRepository<Jobtitle> _JobtitleRepo;
        protected readonly AppDbContext _context;
        public JobtitleService(AppDbContext context, IRepository<Jobtitle> repo)
        {
            _JobtitleRepo = repo;
            _context = context;
        }
        public IEnumerable<Jobtitle> Gets()
        {
            return _JobtitleRepo.Gets(true);
        }
        public void Save(Jobtitle model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Jobtitle entity;

            // CREATE
            if (model.Id == 0)
            {
                entity = new Jobtitle
                {
                    JobtitleName = model.JobtitleName

                };

                _JobtitleRepo.Create(entity);
            }
            // UPDATE
            else
            {
                entity = _JobtitleRepo.Get(model.Id);
                if (entity == null)
                    throw new Exception("Jobtitle không tồn tại");

                entity.JobtitleName = model.JobtitleName;

                _JobtitleRepo.Update(entity);
            }

            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = _JobtitleRepo.Get(id);
            if (entity == null)
                throw new Exception("Jobtitle không tồn tại");

            _JobtitleRepo.Delete(entity);
            _context.SaveChanges();
        }
    }
}
