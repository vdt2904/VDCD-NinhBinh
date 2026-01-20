using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{

    public class PositionService
    {
        private readonly IRepository<Position> _positionRepo;
        protected readonly AppDbContext _context;
        public PositionService(AppDbContext context, IRepository<Position> repo) {
            _positionRepo = repo;
            _context = context;
        }
        public IEnumerable<Position> Gets() {
            return _positionRepo.Gets(true);
        }
        public void Save(Position model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Position entity;

            // CREATE
            if (model.Id == 0)
            {
                entity = new Position
                {
                    PositionName = model.PositionName
                   
                };

                _positionRepo.Create(entity);
            }
            // UPDATE
            else
            {
                entity = _positionRepo.Get(model.Id);
                if (entity == null)
                    throw new Exception("Position không tồn tại");

                entity.PositionName = model.PositionName;

                _positionRepo.Update(entity);
            }

            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = _positionRepo.Get(id);
            if (entity == null)
                throw new Exception("Position không tồn tại");

            _positionRepo.Delete(entity);
            _context.SaveChanges();
        }
    }
}
