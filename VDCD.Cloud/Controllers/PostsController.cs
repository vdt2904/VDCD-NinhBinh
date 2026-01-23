using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Cloud.Controllers;

namespace VDCD.Controllers
{
    public class PostsController : BaseController
    {
        private readonly ILogger<PostsController> _logger;
        private readonly UserBll userService;
        private readonly SettingService _settingService;
        private readonly CenterService _centerService;
        private readonly ProjectService _projectService;
        private readonly CategoryService _categoryService;
        private readonly PostsService _postsService;
        private readonly CustomerService _customerService;
        private readonly UserService _userService;
        private readonly UserDepartmentJobtitlePositionService _userDepartmentJobtitlePositionService;
        private readonly DepartmentService _departmentService;

        public PostsController(ILogger<PostsController> logger, UserBll userBll, SettingService settingService, CenterService centerService
            , SeoMetaService seoMetaService, ProjectService projectService, CategoryService categoryService, PostsService postsService,
            CustomerService customerService, UserService service, UserDepartmentJobtitlePositionService userDepartmentJobtitlePositionService,
            DepartmentService departmentService) : base(seoMetaService)
        {
            _logger = logger;
            userService = userBll;
            _settingService = settingService;
            _centerService = centerService;
            _projectService = projectService;
            _categoryService = categoryService;
            _postsService = postsService;
            _customerService = customerService;
            _userService = service;
            _departmentService = departmentService;
            _userDepartmentJobtitlePositionService = userDepartmentJobtitlePositionService;
        }
        public IActionResult Index(string slug)
        {
            var post = _postsService.GetBySlug(slug);
            if (post == null)
            {
                return NotFound();
            }
            ApplySeo("post:" + post.Slug);
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            ViewBag.LatestPosts = _postsService.GetAll().Take(6);
            return View(post);
        }
    }
}
