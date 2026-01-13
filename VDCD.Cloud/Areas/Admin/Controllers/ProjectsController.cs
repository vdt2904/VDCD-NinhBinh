using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                _projectService.Save(model);
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
    }
}
