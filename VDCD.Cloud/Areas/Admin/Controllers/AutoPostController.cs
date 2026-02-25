using Microsoft.AspNetCore.Mvc;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AutoPostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
