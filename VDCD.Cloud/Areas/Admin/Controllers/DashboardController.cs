using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VDCD.Cloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
