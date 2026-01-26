using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Helper;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
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
                category.Slug = SlugHelper.Generate(category.CategoryName);
                string keywords = GenerateKeywords(category.CategoryName);
                _categoryService.Save(category,keywords);
                return new JsonResult(new {success = true });
            }catch (Exception ex)
            {
                return new JsonResult(new { success = false , message = ex.Message});
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                _categoryService.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        private string GenerateKeywords(string text)
        {
            var slug = SlugHelper.Generate(text);
            return string.Join(", ",
                slug.Split('-')
                    .Where(x => x.Length > 2)
                    .Distinct()
            );
        }
    }
}
