using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class SeoController : Controller
    {
        private readonly SeoMetaService _seoService;

        public SeoController(SeoMetaService seoService)
        {
            _seoService = seoService;
        }
        public IActionResult Index()
        {
            var data = _seoService.GetAll();
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(SeoMeta model)
        {
            try
            {
                _seoService.Save(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            try
            {
                _seoService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
