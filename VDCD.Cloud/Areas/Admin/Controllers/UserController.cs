using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly UserDepartmentJobtitlePositionService _userDepartmentJobtitlePositionService;
        private readonly DepartmentService _departmentService;
        public UserController(UserService userService,UserDepartmentJobtitlePositionService userDepartmentJobtitlePositionService,DepartmentService departmentService) { 
            _userService = userService;
            _userDepartmentJobtitlePositionService = userDepartmentJobtitlePositionService;
            _departmentService = departmentService;
        }
        public IActionResult Index()
        {
            var lst = _userService.GetUsers().ToList();
            return View(lst);
        }
        public IActionResult SearchUsers()
        {
			var lst = _userService.GetUsers().ToList();
			return Json(new { data = lst });
		}
		[HttpGet]
        public IActionResult GetById(int id)
        {
            try
            {
                var user = _userService.GetById(id);
                if (user == null) return Json(new { success = false, message = "Không tìm thấy người dùng" });

                var assignments = _userDepartmentJobtitlePositionService.GetByUserId(id).ToList();
                var resultAssignments = assignments.Select(a => new {
                    a.UserId,
                    a.DepartmentId,
                    // Map tên phòng ban dựa vào ID
                    DepartmentName = _departmentService.Gets().FirstOrDefault(d => d.Id == a.DepartmentId)?.DepartmentName ?? "Phòng " + a.DepartmentId,
                    a.JobtitleId,
                    a.PositionId,
                    a.IsMain
                }).ToList();
                return Json(new
                {
                    success = true,
                    data = user,
                    assignments = resultAssignments
                });
            }
            catch (Exception ex)
            {
                // Log ex ở đây nếu cần
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Save(User user, string SelectedPositionsJson)
        {
            try
            {
                // 1. Chuyển đổi dữ liệu JSON từ View gửi lên
                var assignments = new List<UserDepartmentJobtitlePosition>();
                if (!string.IsNullOrEmpty(SelectedPositionsJson))
                {
                    assignments = JsonConvert.DeserializeObject<List<UserDepartmentJobtitlePosition>>(SelectedPositionsJson);
                }

                // 2. Gán giá trị mặc định tránh lỗi DBNull
                user.IsActive = user.IsActive ?? true;
                user.IsShow = user.IsShow ?? false;

                // 3. Gọi Service xử lý (Đã bao gồm Transaction)
                bool result = _userService.SaveUser(user, assignments);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                _userService.Delete(id);    
                return Json(new {success = true});
            }catch (Exception ex)
            {
                return Json(new { success = false,message = ex.Message});
            }
        }
    }
}
