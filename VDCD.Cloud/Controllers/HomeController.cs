using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VDCD.Business.Service;
using VDCD.Cloud.Models;

namespace VDCD.Cloud.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserBll userService;
        public HomeController(ILogger<HomeController> logger,UserBll userBll)
        {
            _logger = logger;
            userService = userBll;
        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetUsers()
        {
            var lstU = userService.GetAllActiveUsers();
            return Json(lstU);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
