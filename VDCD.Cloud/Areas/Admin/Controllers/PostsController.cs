using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Infrastructure.Filters;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Entities.Enums;
using VDCD.Helper;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class PostsController : Controller
    {
        private readonly PostsService _postsService;
        private readonly CategoryService _categoryService;

        public PostsController(PostsService postsService,
                                 CategoryService categoryService)
        {
            _postsService = postsService;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var lst = _postsService.GetAll();
            ViewBag.Categories = _categoryService.GetAll();
            return View(lst);
        }

        [ActivityLog(ActivityLogType.Post, "Thêm bài viết")]
        public IActionResult Save(Posts model)
        {
            try
            {
                model.Slug = SlugHelper.Generate(model.Title);
                string keywords = GenerateKeywords(model.Title);
                _postsService.Save(model, keywords);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult GetById(int id)
        {
            try
            {
                var model = _postsService.GetById(id);  
                return Json(new { success = true,data = model });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [ActivityLog(ActivityLogType.Post, "Xóa bài viết")]
        public IActionResult Delete(int id)
        {
            try
            {
                _postsService.Delete(id);
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
