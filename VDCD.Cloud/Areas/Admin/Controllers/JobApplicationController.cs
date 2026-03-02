using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VDCD.Business.Service;
using VDCD.Entities.Security;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.HrAccess)]
    public class JobApplicationController : Controller
    {
        private readonly JobApplicationService _jobApplicationService;
        private readonly JobPositionService _jobPositionService;
        public JobApplicationController(JobApplicationService jobApplicationService,JobPositionService jobPositionService)
        {
            _jobPositionService=jobPositionService;
            _jobApplicationService = jobApplicationService;
        }
        public IActionResult Index()
        {
            ViewBag.Job = _jobPositionService.GetAll();
            var lst = _jobApplicationService.GetAll();
            return View(lst);
        }
        public IActionResult GetById(int id) {

            var cv = _jobApplicationService.GetById(id);
            return Json(cv);
        }
        public JsonResult UpdateStatus(int id,string review, string status)
        {
            var item = _jobApplicationService.GetById(id);
            if (item != null)
            {
                item.Review = review;
                item.Status = status;
                _jobApplicationService.Review(item);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _jobApplicationService.Delete(id);
            return Json(new { success = false });
        }
    }
}
