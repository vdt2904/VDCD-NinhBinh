using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Entities.Security;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.HrAccess)]
    public class PositionController : Controller
    {
        private readonly PositionService _positionService;
        public PositionController(PositionService positionService) { 
            _positionService = positionService;
        }
        public IActionResult Index()
        {
            var list = _positionService.Gets().ToList();
            return View(list);
        }


        public IActionResult Save(Position model)
        {
            try
            {
                _positionService.Save(model);
                return Json(new {success = true});
            }catch (Exception ex)
            {
                return Json(new {success = false,error = ex.Message});
            }
        }

        // Xóa
        public IActionResult Delete(int id)
        {
            try
            {
                _positionService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
