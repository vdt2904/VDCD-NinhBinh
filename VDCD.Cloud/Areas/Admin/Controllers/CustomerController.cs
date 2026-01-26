using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class CustomerController : Controller
    {
        private readonly CustomerService _customerService;
        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }
        public IActionResult Index()
        {
            var data = _customerService.GetAll();
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Customer model)
        {
            try
            {
                _customerService.Save(model);
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
            _customerService.Delete(id);
            return Json(new { success = true });
        }
    }
}
