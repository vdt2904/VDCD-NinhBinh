using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;

namespace VDCD.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly SeoMetaService _seoMetaService;
        private readonly SettingService _settingService;
        private readonly CategoryService _categoryService;
        private readonly ProjectService _projectService;
        public CategoryController(SeoMetaService seoMetaService, SettingService settingService, CategoryService categoryService,
            ProjectService projectService) : base(seoMetaService)
        {
            _seoMetaService = seoMetaService;
            _settingService = settingService;
            _categoryService = categoryService;
            _projectService = projectService;
        }
        public IActionResult Index(string slug)
        {
            var cat = _categoryService.GetBySlug(slug);
            if(cat == null)
            {
                return NotFound();
            }
            ApplySeo("category:"+cat.Slug);
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            return View(cat);
        }
        public IActionResult ListProject(int Id) {
            try
            {
                var lst = _projectService.GetAll().Where(x => x.CategoryId == Id);
                return Json(new { success = true, data = lst });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
    }
}
