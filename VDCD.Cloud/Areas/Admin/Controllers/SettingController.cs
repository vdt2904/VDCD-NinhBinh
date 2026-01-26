using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Cloud.Controllers;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class SettingController : Controller
    {
        private readonly SettingService _settingService;
        private readonly ILogger<SettingController> _logger;
        private readonly UserBll userService;
        public SettingController(ILogger<SettingController> logger, UserBll userBll,SettingService settingService)
        {
            _logger = logger;
            userService = userBll;
            _settingService = settingService;
        }
        public IActionResult GeneralSetting()
        {
            var lstSetting = _settingService.GetAll();
            ViewBag.Footer = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.footer."));
            ViewBag.Social = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.social."));
            ViewBag.Trademark = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.trademark."));
            ViewBag.Strengths = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.strengths."));
            ViewBag.Sliders = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.sliders."));
            ViewBag.AboutUs = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.aboutus."));
            ViewBag.Visions = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.vision."));
            ViewBag.Services = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.services."));
			ViewBag.Operations = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.operation."));
			ViewBag.OrgStructure = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.OrgStructure."));
			ViewBag.Solutions = lstSetting.Where(x => x.SettingKey.StartsWith("setting.general.solution."));
			ViewBag.EmailSettings = lstSetting.Where(x => x.SettingKey.StartsWith("setting.email."));
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult save(IFormCollection form)
        {
            _settingService.DeleteByPrefix("setting.general.strengths.");
            _settingService.DeleteByPrefix("setting.general.vision.");
            _settingService.DeleteByPrefix("setting.general.solution.");
            foreach (var key in form.Keys)
            {
                // bỏ token
                if (key == "__RequestVerificationToken")
                    continue;

                var value = form[key].ToString();

                _settingService.Save(key, value);
            }
            _settingService.Comit();
            TempData["Success"] = "Lưu cấu hình thành công";
            return new JsonResult(new { success = true, message = "Lưu cấu hình thành công" }) ;
        }

    }
}
