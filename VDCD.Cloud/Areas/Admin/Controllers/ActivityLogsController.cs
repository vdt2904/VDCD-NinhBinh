using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Infrastructure;
using VDCD.Entities.DTO;

namespace VDCD.Areas.Admin.Controllers
{
    [Route("api/activity-logs")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _service;

        public ActivityLogsController(IActivityLogService service)
        {
            _service = service;
        }

        // ⭐ GET /api/activity-logs?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> Get(
            int page = 1,
            int pageSize = 20)
        {
            var result =
                await _service.GetPagedAsync(page, pageSize);

            return Ok(result);
        }

        // ⭐ POST /api/activity-logs/search
        [HttpPost("search")]
        public async Task<IActionResult> Search(
            ActivityLogSearchRequest req)
        {
            var result =
                await _service.SearchAsync(req);

            return Ok(result);
        }
    }
}
