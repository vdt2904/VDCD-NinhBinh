using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Entities.Security;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.HrAccess)]
    public class JobtitleController : Controller
    {
        private readonly JobtitleService _JobtitleService;
        public JobtitleController(JobtitleService JobtitleService)
        {
            _JobtitleService = JobtitleService;
        }
        public IActionResult Index()
        {
            var list = _JobtitleService.Gets().ToList();
            return View(list);
        }


        public IActionResult Save(Jobtitle model)
        {
            try
            {
                _JobtitleService.Save(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Xóa
        public IActionResult Delete(int id)
        {
            try
            {
                _JobtitleService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
