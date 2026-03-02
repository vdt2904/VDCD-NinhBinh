using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Entities.Security;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.SuperAdminOnly)]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly UserRoleService _userRoleService;
        private readonly UserDepartmentJobtitlePositionService _userDepartmentJobtitlePositionService;
        private readonly DepartmentService _departmentService;

        public UserController(
            UserService userService,
            UserRoleService userRoleService,
            UserDepartmentJobtitlePositionService userDepartmentJobtitlePositionService,
            DepartmentService departmentService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
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
                    roleName = _userRoleService.GetRoleNameByUserId(id),
                    assignments = resultAssignments
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Save(User user, string SelectedPositionsJson, string roleName, string? newPassword)
        {
            try
            {
                var assignments = new List<UserDepartmentJobtitlePosition>();
                if (!string.IsNullOrEmpty(SelectedPositionsJson))
                {
                    assignments = JsonConvert.DeserializeObject<List<UserDepartmentJobtitlePosition>>(SelectedPositionsJson)
                                  ?? new List<UserDepartmentJobtitlePosition>();
                }

                user.IsActive = user.IsActive ?? true;
                user.IsShow = user.IsShow ?? false;

                bool result = _userService.SaveUser(user, assignments, roleName, newPassword);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _userService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
