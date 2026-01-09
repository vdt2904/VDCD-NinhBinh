using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Cloud.Controllers;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return View();
        }
        public JsonResult getsetting(string settingName)
        {
            var lstSetting = _settingService.GetAll();
            return Json(lstSetting);
        }
    }
}
