using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class DepartmentController : Controller
    {
        private readonly DepartmentService _departmentService;
        private readonly UserService _userService;
        private readonly JobtitleService _jobtitleService;
        private readonly PositionService _positionService;
        public DepartmentController(DepartmentService departmentService,UserService userService,
			JobtitleService jobtitleService,PositionService positionService)
        {
            _departmentService = departmentService;
            _userService = userService;
			_jobtitleService = jobtitleService;
			_positionService = positionService;
        }

        public IActionResult Index()
        {
            var departments = _departmentService.Gets().ToList();
            return View(departments);
        }
        public IActionResult GetTreeData()
        {
            var departments = _departmentService.Gets().ToList();
            var treeNodes = departments.Select(d => new
            {
                id = d.Id.ToString(),
                parent = d.ParentId == null ? "#" : d.ParentId.ToString(),
                text = d.DepartmentName,
                icon = "bi bi-folder-fill text-warning"
            });
            return Json(treeNodes);
        }
		[HttpGet]
		public IActionResult GetDetailWithUsers(int id)
		{
			try
			{
				var dept = _departmentService.GetById(id);
				var udjp = _departmentService.GetUsersInDept(id);
				var userIds = _userService.GetUsers();
				var users = udjp
	.Where(x => x.DepartmentId == id)
	.GroupJoin(
		userIds,
		ud => ud.UserId,
		u => u.UserId,
		(ud, u) => new { ud, u }
	)
	.SelectMany(
		x => x.u.DefaultIfEmpty(),
		(x, u) => new
		{
			Id = x.ud.Id,
			UserId = x.ud.UserId,
			Fullname = u != null ? u.FullName : null,
			DepartmentId = x.ud.DepartmentId,
			JobtitleId = x.ud.JobtitleId,
			PositionId = x.ud.PositionId,
			IsMain = x.ud.IsMain,
		}
	);


				return Json(new { success = true, data = dept, users = users });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}
		[HttpGet]
		public IActionResult GetJobtitles()
		{
			// Lấy danh sách từ Service, xử lý null nếu cần
			var data = _jobtitleService.Gets()
						.Select(x => new { id = x.Id, name = x.JobtitleName })
						.ToList();
			return Json(data);
		}

		[HttpGet]
		public IActionResult GetPositions()
		{
			var data = _positionService.Gets()
						.Select(x => new { id = x.Id, name = x.PositionName })
						.ToList();
			return Json(data);
		}
        [HttpPost]
        public IActionResult Save(Department dept, string AssignmentsJson)
        {
            try
            {
                // Giải mã JSON ngay tại Controller hoặc truyền chuỗi vào Service tùy bạn
                var assignments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserDepartmentJobtitlePosition>>(AssignmentsJson);

                // Gọi Service xử lý
                var result = _departmentService.SaveDepartmentWithUsers(dept, assignments);

                if (result)
                    return Json(new { success = true, message = "Lưu thành công!" });
                else
                    return Json(new { success = false, message = "Lưu thất bại." });
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
                _departmentService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
			{
				return Json(new {success = false, message = ex.Message});
			}

		}
    }
}
