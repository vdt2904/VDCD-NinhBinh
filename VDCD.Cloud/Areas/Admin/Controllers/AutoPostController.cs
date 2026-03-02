using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Security;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.ContentAccess)]
    public class AutoPostController : Controller
    {
        private UserService _userService;
        public AutoPostController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            ViewBag.ListUser = _userService.GetUsers();
            return View();
        }
    }
}
