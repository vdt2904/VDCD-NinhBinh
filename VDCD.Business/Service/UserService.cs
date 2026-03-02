using VDCD.Business.Security;
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
        private readonly UserRoleService _userRoleService;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public UserService(IRepository<User> Repo,
                           ICacheService cache,
                           AppDbContext context,
                           IRepository<UserDepartmentJobtitlePosition> udjpRepo,
                           UserRoleService userRoleService)
        {
            _userRepo = Repo;
            _cache = cache;
            _context = context;
            _udjpRepo = udjpRepo;
            _userRoleService = userRoleService;
        }

        public IEnumerable<User> GetUsers()
        {
            if (_cache.TryGet(CacheParam.UsersAll, out List<User> cached))
            {
                ApplyRoleNames(cached);
                return cached;
            }

            var data = _userRepo
                .GetsReadOnly()
                .OrderByDescending(x => x.UserId)
                .ToList();

            _cache.Set(
                CacheParam.UsersAll,
                data,
                TimeSpan.FromMinutes(CacheParam.UsersAllTimeout)
            );

            ApplyRoleNames(data);
            return data ?? Enumerable.Empty<User>();
        }

        public User GetById(int id)
        {
            var user = _userRepo.Get(true, x => x.UserId == id);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            user.RoleName = _userRoleService.GetRoleNameByUserId(user.UserId);
            return user;
        }

        public bool SaveUser(User user, List<UserDepartmentJobtitlePosition> assignments, string? roleName, string? newPassword)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
                throw new Exception("Tài khoản không được để trống.");

            user.UserName = user.UserName.Trim();
            if (_userRepo.Exist(x => x.UserId != user.UserId && x.UserName == user.UserName))
                throw new Exception("Tài khoản đã tồn tại.");

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (user.UserId == 0)
                    {
                        if (string.IsNullOrWhiteSpace(newPassword))
                            throw new Exception("Cần nhập mật khẩu khi tạo tài khoản mới.");

                        user.CreateAt = DateTime.Now;
                        user.HashPassword = PasswordSecurity.HashPassword(newPassword.Trim());
                        _userRepo.Create(user);
                    }
                    else
                    {
                        var dbUser = _userRepo.Get(false, x => x.UserId == user.UserId);
                        if (dbUser == null)
                            throw new Exception("Nhân sự không tồn tại.");

                        dbUser.UserName = user.UserName;
                        dbUser.FullName = user.FullName;
                        dbUser.Mail = user.Mail;
                        dbUser.Phone = user.Phone;
                        dbUser.IsActive = user.IsActive;
                        dbUser.Profile = user.Profile;
                        dbUser.IsShow = user.IsShow;
                        dbUser.Avatar = user.Avatar;
                        dbUser.CreateAt = dbUser.CreateAt ?? user.CreateAt;

                        if (!string.IsNullOrWhiteSpace(newPassword))
                            dbUser.HashPassword = PasswordSecurity.HashPassword(newPassword.Trim());

                        _userRepo.Update(dbUser);
                        user = dbUser;
                    }

                    _context.SaveChanges();

                    _userRoleService.UpsertRole(user.UserId, roleName, false);

                    var oldAssignments = _udjpRepo.Gets(false, x => x.UserId == user.UserId).ToList();
                    if (oldAssignments.Any())
                    {
                        _udjpRepo.DeleteRange(oldAssignments);
                    }

                    if (assignments != null && assignments.Count > 0)
                    {
                        foreach (var item in assignments)
                        {
                            if (item.JobtitleId == 0) item.JobtitleId = null;
                            if (item.UserId == 0) item.UserId = null;
                            if (item.PositionId == 0) item.PositionId = null;
                            if (item.DepartmentId == 0) item.DepartmentId = null;
                            item.UserId = user.UserId;
                            _udjpRepo.Create(item);
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    ClearCache();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
        }

        private void ClearCache()
        {
            _cache.Remove(CacheParam.UsersAll);
        }

        public void Delete(int id)
        {
            var user = _userRepo.Get(false, x => x.UserId == id);
            if (user == null) { throw new Exception("Nhân viên không tồn tại"); }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var udjp = _udjpRepo.Gets(false, x => x.UserId == id).ToList();
                    if (udjp.Any())
                        _udjpRepo.DeleteRange(udjp);

                    _userRoleService.DeleteByUserId(id, false);
                    _userRepo.Delete(user);
                    _context.SaveChanges();

                    transaction.Commit();
                    ClearCache();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public User? FindByUsername(string? username, bool onlyActive = false)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            username = username.Trim();

            return _userRepo.Get(true, x =>
                x.UserName == username &&
                (!onlyActive || x.IsActive == true));
        }

        public bool VerifyPassword(User user, string? password)
        {
            if (user == null)
                return false;

            return PasswordSecurity.VerifyPassword(user.HashPassword, password);
        }

        private void ApplyRoleNames(IEnumerable<User> users)
        {
            if (users == null)
                return;

            var roleMap = _userRoleService.GetRoleMapByUserId();
            foreach (var user in users)
            {
                if (user == null)
                    continue;

                user.RoleName = roleMap.TryGetValue(user.UserId, out var role) ? role : _userRoleService.NormalizeRole(null);
            }
        }

        public User GetByUsername(string username)
        {
            var user = _userRepo.Get(true, x => x.UserName == username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(username));
            }
            return user;
        }
    }
}
