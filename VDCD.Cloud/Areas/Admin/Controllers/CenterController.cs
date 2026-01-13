using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CenterController : Controller
    {
        private readonly CenterService _centerService;
        public CenterController(CenterService centerService) => _centerService = centerService;

        public IActionResult Index()
        {
            var data = _centerService.GetAll();
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Center model)
        {
            try
            {
                _centerService.Save(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _centerService.Delete(id);
            return Json(new { success = true });
        }
    }
}
