using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VDCD.Cloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // TODO: check DB
            if (username != "admin" || password != "123")
            {
                ViewBag.Error = "Sai tài khoản";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(
                claims,
                "AdminAuth");

            await HttpContext.SignInAsync(
                "AdminAuth",
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Login");
        }
    }
}
