using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly UserBll userService;
        public CategoryController(ILogger<CategoryController> logger, UserBll userBll, CategoryService categoryService)
        {
            _logger = logger;
            userService = userBll;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var lst = _categoryService.GetAll();
            return View(lst);
        }
        public IActionResult Save(Category category)
        {
            try
            {
                _categoryService.Save(category);
                return new JsonResult(new {success = true });
            }catch (Exception ex)
            {
                return new JsonResult(new { success = false , message = ex.Message});
            }
        }
    }
}
