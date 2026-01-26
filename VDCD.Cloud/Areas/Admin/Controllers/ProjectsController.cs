using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Helper;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(AuthenticationSchemes = "AdminAuth")]
	public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly CategoryService _categoryService;

        public ProjectsController(ProjectService projectService,
                                 CategoryService categoryService)
        {
            _projectService = projectService;
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            var lst = _projectService.GetAll();
            ViewBag.Categories = _categoryService.GetAll();
            return View(lst);
        }
        [HttpPost]
        public IActionResult Save(Project model)
        {
            try
            {
                model.Plug = SlugHelper.Generate(model.ProjectName);
                string keywords = GenerateKeywords(model.ProjectName);
                _projectService.Save(model,keywords);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                _projectService.Delete(id);
                return Json(new { success = true });
            }catch (Exception ex)
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
