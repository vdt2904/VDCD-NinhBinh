using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VDCD.Business.Infrastructure;
using VDCD.DataAccess;
using VDCD.Entities.Cache;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class UserService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<UserDepartmentJobtitlePosition> _udjpRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public UserService(IRepository<User>Repo,
                              ICacheService cache,
                              AppDbContext context,


                              IRepository<UserDepartmentJobtitlePosition> udjpRepo)
        {
            _userRepo = Repo;
            _cache = cache;
            _context = context;
            _udjpRepo = udjpRepo;
        }
        public IEnumerable<User> GetUsers()
        {
            if (_cache.TryGet(CacheParam.UsersAll, out List<User> cached))
                return cached;

            var data = _userRepo
                .GetsReadOnly()
                .OrderByDescending(x => x.UserId)
                .ToList();

            _cache.Set(
                CacheParam.UsersAll,
                data,
                TimeSpan.FromMinutes(CacheParam.UsersAllTimeout)
            );

            return data ?? Enumerable.Empty<User>(); ;
        }

        public User GetById(int id)
        {
            var user = _userRepo.Get(true, x => x.UserId == id);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            return user;
        }
        public bool SaveUser(User user, List<UserDepartmentJobtitlePosition> assignments)
        {
            // Bắt đầu Transaction
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (user.UserId == 0)
                    {
                        user.CreateAt = DateTime.Now;
                        _userRepo.Create(user);
                    }
                    else
                    {
                        _userRepo.Update(user);
                        // Xóa các phân quyền cũ trực tiếp bằng Context
                        var oldEntries = _udjpRepo.Gets(false,x=>x.UserId == user.UserId)
                                                 .Where(x => x.UserId == user.UserId);                       
                    }

                    // Lưu User trước để lấy UserId (trong trường hợp Insert)
                    _context.SaveChanges();

                    // Lưu bảng trung gian
                    if (assignments != null && assignments.Count > 0)
                    {
                        foreach (var item in assignments)
                        {
                            item.UserId = user.UserId; // Đảm bảo lấy đúng UserId mới sinh
                            _udjpRepo.Create(item);
                        }
                        _context.SaveChanges();
                    }

                    // Nếu mọi thứ OK, Commit dữ liệu
                    transaction.Commit();
                    ClearCache();
                    return true;
                }
                catch (Exception ex)
                {
                    // Nếu có bất kỳ lỗi nào, Rollback (hủy bỏ) toàn bộ quá trình
                    transaction.Rollback();
                    throw new Exception("Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
        }
        private void ClearCache()
        {
            _cache.Remove(CacheParam.UsersAll);
        }
    }
}
