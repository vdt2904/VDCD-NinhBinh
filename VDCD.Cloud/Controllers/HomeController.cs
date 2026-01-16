using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VDCD.Business.Service;
using VDCD.Cloud.Models;
using VDCD.Controllers;
using VDCD.Models;

namespace VDCD.Cloud.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserBll userService;
        private readonly SettingService _settingService;
        private readonly CenterService _centerService;
        private readonly ProjectService _projectService;
        private readonly CategoryService _categoryService;  
        private readonly PostsService _postsService;  
        private readonly CustomerService _customerService;
        public HomeController(ILogger<HomeController> logger,UserBll userBll, SettingService settingService,CenterService centerService
            , SeoMetaService seoMetaService, ProjectService projectService, CategoryService categoryService,PostsService postsService,
            CustomerService customerService) : base(seoMetaService)
        {
            _logger = logger;
            userService = userBll;
            _settingService = settingService;   
            _centerService = centerService;
            _projectService = projectService;
            _categoryService = categoryService;
            _postsService = postsService;
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            ApplySeo("home");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var homeModel = new HomeModelView
            {
                Settings = settingsDic, // Gán vào một thuộc tính Dictionary trong ViewModel
                Centers = _centerService.GetAll().ToList(),
                Projects = _projectService.GetAll().ToList(),
                Blogs = _postsService.GetAll().Take(6).ToList(),
                Customers = _customerService.GetAll().Where(x=>x.IsShow == true).ToList(),
            };

            return View(homeModel);
        }
        public IActionResult Center()
        {
            ApplySeo("he-thong-trung-tam");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var Data = new CenterModelView
            {
                Settings = settingsDic,
                Centers = _centerService.GetAll().ToList(),
            };
            return View(Data);
        }
        public JsonResult GetUsers()
        {
            var lstU = userService.GetAllActiveUsers();
            return Json(lstU);
        }
        public JsonResult GetHeader()
        {
            var lstSetting = _settingService.GetAll();


            var footer = lstSetting
                .Where(x => x.SettingKey.StartsWith("setting.general.footer."))
                .OrderBy(x => x.SettingKey)
                .ToList();

            var slice = lstSetting
                .Where(x => x.SettingKey.StartsWith("setting.general.sliders."))
                .AsEnumerable() // Chuyển về bộ nhớ để xử lý chuỗi
                .OrderBy(x => {
                    var parts = x.SettingKey.Split('.');
                    int index;
                    return int.TryParse(parts.Last(), out index) ? index : 0;
                })
                .ToList();

            return Json(new
            {
                success = true,
                footer = footer,
                slice = slice
            });
        }
    }
}
