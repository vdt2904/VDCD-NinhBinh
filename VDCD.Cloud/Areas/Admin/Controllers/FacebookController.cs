using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Service;
using VDCD.Entities.DTO;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FacebookController : Controller
    {
        private readonly DepartmentService _departmentService;
        private readonly UserService _userService;
        private readonly JobtitleService _jobtitleService;
        private readonly PositionService _positionService;
        private readonly FacebookService _facebookService;
        public FacebookController(DepartmentService departmentService, UserService userService,
            JobtitleService jobtitleService, PositionService positionService, FacebookService facebookService)
        {
            _departmentService = departmentService;
            _userService = userService;
            _jobtitleService = jobtitleService;
            _positionService = positionService;
            _facebookService = facebookService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Posts([FromBody] FacebookPostRequest model)
        {
            if (model == null)
                return BadRequest("Model is null");

            try
            {
                object result;

                bool hasImages = model.ImageUrls != null && model.ImageUrls.Any();
                bool hasVideos = model.Videos != null && model.Videos.Any();

                if (!hasImages && !hasVideos)
                {
                    result = await _facebookService.PostTextAsync(
                        model.PageId,
                        model.Messgase,
                        model.Token
                    );
                }
                else if (hasVideos)
                {
                    result = await _facebookService.PostVideoAsync(
                        model.PageId,
                        model.Videos,
                        model.Messgase,
                        model.Token
                    );
                }
                else
                {
                    result = await _facebookService.PostImagesAsync(
                        model.PageId,
                        model.ImageUrls,
                        model.Messgase,
                        model.Token
                    );
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log ra để debug (quan trọng)
                // _logger.LogError(ex, "Error while posting to Facebook");

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
