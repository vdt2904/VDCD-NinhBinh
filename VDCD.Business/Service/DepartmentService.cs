using Microsoft.EntityFrameworkCore;
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
    public class DepartmentService
    {
        private readonly IRepository<Department> _depRepo;
        private readonly IRepository<UserDepartmentJobtitlePosition> _udjpRepo;
        private readonly ICacheService _cache;
        protected readonly AppDbContext _context;
        public DepartmentService(AppDbContext context,ICacheService cache,IRepository<Department> repo,IRepository <UserDepartmentJobtitlePosition> udjp) {
            _cache = cache;
            _depRepo = repo;
            _context = context;
            _udjpRepo = udjp;
        }

        public IEnumerable<Department> Gets()
        {
            return _depRepo.Gets(true);
        }

		public Department GetById(int id)
		{
			var dep = _depRepo.Get(false,x=>x.Id == id);
            if(dep == null)
				throw new Exception("Phòng ban không tồn tại");
            return dep;
		}

		public IEnumerable<UserDepartmentJobtitlePosition> GetUsersInDept(int id)
		{
			var lst = _udjpRepo.Gets(true,x=>x.DepartmentId == id);
            return lst ?? Enumerable.Empty<UserDepartmentJobtitlePosition>();
        }

        public bool SaveDepartmentWithUsers(Department dept, List<UserDepartmentJobtitlePosition> assignments)
        {
            // Sử dụng Transaction từ DbContext
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var parent = _depRepo.Get(true,x=>x.Id == dept.ParentId);
                    // 1. Lưu/Cập nhật thông tin phòng ban
                    if (dept.Id == 0)
                    {
                        _depRepo.Create(dept);
                    }
                    else
                    {
                        _depRepo.Update(dept);
                    }
                    _context.SaveChanges(); // Lưu để lấy ID nếu là phòng ban mới
                    if (parent != null)
                    {
                        dept.DepartmentExt = parent.DepartmentExt + "." + dept.Id.ToString();
                        dept.DepartmentPath = parent.DepartmentPath + "/" + dept.DepartmentName;
                        _depRepo.Update(dept);
                        _context.SaveChanges();
                    }
                    // 2. Xử lý nhân sự: Xóa hết các bản ghi cũ của phòng ban này
                    var oldAssignments = _udjpRepo.Gets(false,x => x.DepartmentId == dept.Id).ToList();
                    if (oldAssignments.Any())
                    {
                        _udjpRepo.DeleteRange(oldAssignments);
                    }

                    // 3. Thêm danh sách nhân sự mới
                    if (assignments != null && assignments.Count > 0)
                    {
                        foreach (var item in assignments)
                        {
                            item.Id = 0; // Đảm bảo tạo bản ghi mới hoàn toàn
                            item.DepartmentId = dept.Id;
                            _udjpRepo.Create(item);
                        }
                    }

                    _context.SaveChanges();

                    // 4. Hoàn tất giao dịch
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    // Nếu có bất kỳ lỗi nào, rollback lại từ đầu
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void Delete(int id)
        {
            var lst = _depRepo.Gets(false,x=>(x.ParentId == id) || x.Id == id);
            foreach (var item in lst)
            {
                _udjpRepo.DeleteRange(_udjpRepo.Gets(false,x=>x.DepartmentId == item.Id));
            }
            _context.SaveChanges();
            _depRepo.DeleteRange(lst);
            _context.SaveChanges();
        }
    }
}
