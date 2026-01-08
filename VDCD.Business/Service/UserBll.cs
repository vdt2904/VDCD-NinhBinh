using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.DataAccess;
using VDCD.Entities.Custom;

namespace VDCD.Business.Service
{
    public class UserBll
    {
        private readonly IRepository<User> _userRepo;
        public UserBll(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }
        public List<User> GetAllActiveUsers()
        {
            return _userRepo.GetsReadOnly(u => (bool)u.IsActive).ToList();
        }

        // Lấy user theo Id (read-write)
        public User GetUserById(int id)
        {
            return _userRepo.Get(false, u => u.UserId == id);
        }

        // Tạo user mới
        public void CreateUser(User user)
        {
            _userRepo.Create(user);
        }

        // Xóa user
        public void DeleteUser(User user)
        {
            _userRepo.Delete(user);
        }

        // Kiểm tra tồn tại
        public bool UserExists(int id)
        {
            return _userRepo.Exist(u => u.UserId == id);
        }

        // Đếm tổng số user
        public int CountUsers()
        {
            return _userRepo.Count();
        }
    }
}
