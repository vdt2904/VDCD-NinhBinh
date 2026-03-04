using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Infrastructure;
using VDCD.Business.Service;
using VDCD.Entities.DTO;
using VDCD.Entities.Enums;
using VDCD.Helper;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class ActivityLogsController : Controller
    {
        private readonly IActivityLogService _service;

        public ActivityLogsController(IActivityLogService service)
        {
            _service = service;
        }

        // GET: /Admin/ 
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            ViewBag.TypeList = GetActivityLogTypes();
            return View(result);
        }

         // GET: /Admin/ActivityLogs/Search
        [HttpGet]
        public async Task<IActionResult> Search(
            string? Content,
            string? TypeText,
            int page = 1,
            int pageSize = 10)
        {
            var req = new ActivityLogSearchRequest
            {
                Content = Content,
                TypeText = TypeText,
                Page = page,
                PageSize = pageSize
            };

            var result = await _service.SearchAsync(req);

            ViewBag.Content = Content;
            ViewBag.TypeText = TypeText;
            ViewBag.TypeList = GetActivityLogTypes();

            return View("Index", result);
        }

        private List<string> GetActivityLogTypes()
        {
            return Enum.GetValues(typeof(ActivityLogType))
                .Cast<ActivityLogType>()
                .Select(x => x.GetDescription())
                .ToList();
        }
    }
}
