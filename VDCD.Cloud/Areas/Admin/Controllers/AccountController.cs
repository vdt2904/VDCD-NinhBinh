using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VDCD.Business.Service;

namespace VDCD.Cloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly CacheSevice _cacheSevice;
        private readonly UserService _userService;

        public AccountController(CacheSevice cacheSevice, UserService userService) { 
            _cacheSevice = cacheSevice;
            _userService = userService;
        }
        [HttpGet]
		public IActionResult Login(string returnUrl = "/admin")
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/admin")
        {
            // TODO: check DB
            if (username != "admin" || password != "123abc@A!")
            {
                ViewBag.Error = "Sai tài khoản";
                return View();
            }

            var user = _userService.GetByUserName(username);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(
                claims,
                "AdminAuth");

            await HttpContext.SignInAsync(
                "AdminAuth",
                new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return Redirect("/Admin");
        }
        public IActionResult RemoveCache()
        {
            _cacheSevice.ClearAll();
            return  Redirect("/Admin");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Login");
        }
    }
}
