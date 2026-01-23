using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Cloud.Controllers;
using VDCD.Models;

namespace VDCD.Controllers
{
    public class ProjectsController : BaseController
    {
        private readonly ILogger<ProjectsController> _logger;
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

        public ProjectsController(ILogger<ProjectsController> logger, UserBll userBll, SettingService settingService, CenterService centerService
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
        
		public IActionResult Index(int page = 1)
		{
			ApplySeo("du-an");
			var lstSetting = _settingService.GetAll();
			var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);

			// Cấu hình phân trang
			int pageSize = 9;
			var allProjects = _projectService.GetAll().ToList();
			var totalProjects = allProjects.Count;
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            // Lấy dữ liệu của trang hiện tại
            var projectsPaged = allProjects
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var homeModel = new ProjectsViewModel
			{
				Settings = settingsDic,
				Projects = projectsPaged, // Chỉ gửi các project của trang hiện tại
				Categories = _categoryService.GetAll().ToList(),
			};

			// Truyền dữ liệu phân trang qua ViewBag để View hiển thị
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalProjects / pageSize);

			return View(homeModel);
		}
        public IActionResult Project(string slug)
        {
            var prj = _projectService.GetBySlug(slug);
            if (prj == null)
            {
                return NotFound();
            }
            ApplySeo("project:" + prj.Plug);
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            return View(prj);
        }
	}
}
