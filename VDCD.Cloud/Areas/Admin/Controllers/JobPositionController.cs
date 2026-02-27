using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Entities.Security;
using VDCD.Helper;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.HrAccess)]
    public class JobPositionController : Controller
    {
        private readonly JobPositionService _jobPositionService;
        public JobPositionController(JobPositionService jobPositionService)
        {
            _jobPositionService = jobPositionService;
        }
        public IActionResult Index()
        {
            var lst = _jobPositionService.GetAll();
            return View(lst);
        }
        [HttpPost]
        public IActionResult Save(JobPosition model)
        {
            try
            {
                model.Slug = SlugHelper.Generate(model.Title);
                var keywords = GenerateKeywords(model.Title);
                _jobPositionService.Save(model, keywords);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult GetById(int id)
        {
            try
            {
                var data = _jobPositionService.GetById(id);
                return Json(new { success = true , data = data });
            }catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult Delete(int id) {
            try
            {
                _jobPositionService.Delete(id);
                return Json(new { success = true, message = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private string GenerateKeywords(string text)
        {
            var slug = SlugHelper.Generate(text);
            return string.Join(", ",
                slug.Split('-')
                    .Where(x => x.Length > 2)
                    .Distinct()
            );
        }
    }
}
