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
    public class UserDepartmentJobtitlePositionService
    {
        private readonly IRepository<UserDepartmentJobtitlePosition> _userDepartmentJobtitlePositionRepository;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public UserDepartmentJobtitlePositionService(AppDbContext dbContext,ICacheService cache,IRepository<UserDepartmentJobtitlePosition> repository)
        {
            _userDepartmentJobtitlePositionRepository = repository;
            _cache = cache;
            _context = dbContext;
        }
        public IEnumerable<UserDepartmentJobtitlePosition> GetByUserId(int id) {
            return _userDepartmentJobtitlePositionRepository.Gets(true,x=>x.UserId == id);
        }
    }
}
