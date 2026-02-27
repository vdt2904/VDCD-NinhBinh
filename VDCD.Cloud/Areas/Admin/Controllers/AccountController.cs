using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VDCD.Business.Service;
using VDCD.Entities.Security;
using VDCD.Extensions;
namespace VDCD.Cloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly CacheSevice _cacheSevice;
        private readonly UserService _userService;
        private readonly UserRoleService _userRoleService;
        private readonly RbacDemoSeedService _rbacDemoSeedService;

        public AccountController(
            CacheSevice cacheSevice,
            UserService userService,
            UserRoleService userRoleService,
            RbacDemoSeedService rbacDemoSeedService)
        {
            _cacheSevice = cacheSevice;
            _userService = userService;
            _userRoleService = userRoleService;
            _rbacDemoSeedService = rbacDemoSeedService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = "/admin")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/admin")
        {
            username = username?.Trim() ?? string.Empty;
            password ??= string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập tài khoản và mật khẩu";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var foundUser = _userService.FindByUsername(username, onlyActive: true);
            var isLegacyAdmin = foundUser == null &&
                                string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase) &&
                                password == "123abc@A!";

            if (foundUser == null && !isLegacyAdmin)
            {
                ViewBag.Error = "Sai tài khoản";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            if (foundUser != null && !_userService.VerifyPassword(foundUser, password))
            {
                ViewBag.Error = "Sai mật khẩu";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var roleName = foundUser != null
                ? _userRoleService.GetRoleNameByUserId(foundUser.UserId)
                : AdminRoles.SuperAdmin;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, foundUser?.UserId.ToString() ?? "0"),
                new Claim(ClaimTypes.Name, foundUser?.UserName ?? username),
                new Claim(ClaimTypes.Role, roleName)
            };

            if (!string.IsNullOrWhiteSpace(foundUser?.FullName))
                claims.Add(new Claim(ClaimTypes.GivenName, foundUser.FullName!));

            var identity = new ClaimsIdentity(claims, "AdminAuth");
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync("AdminAuth", principal, authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return Redirect("/Admin");
        }

        [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.SuperAdminOnly)]
        public IActionResult RemoveCache()
        {
            _cacheSevice.ClearAll();
            return Redirect("/Admin");
        }

        [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.SuperAdminOnly)]
        [HttpPost("/admin/account/seed-demo-rbac")]
        public IActionResult SeedDemoRbac(bool resetPassword = false)
        {
            try
            {
                var result = _rbacDemoSeedService.SeedDemoUsers(resetPassword);
                return Json(new
                {
                    success = true,
                    message = "Seed dữ liệu demo RBAC thành công.",
                    createdUsers = result.CreatedUsers,
                    updatedUsers = result.UpdatedUsers,
                    accounts = result.Accounts
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Seed dữ liệu demo RBAC thất bại.",
                    error = ex.Message
                });
            }
        }

        [Authorize(AuthenticationSchemes = "AdminAuth")]
        [HttpGet("/admin/account/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet("/Admin/Account/Denied")]
        public IActionResult Denied()
        {
            return View();
        }
        public IActionResult GetCurrentUser()
        {
            var user = new
            {
                UserId = User.GetUserId(),
                UserName = User.GetUsername(),
                UserRole = User.GetRole(),
                FullName = User.GetFullName(),
			};
            return Ok(user);
        }
    }
}
