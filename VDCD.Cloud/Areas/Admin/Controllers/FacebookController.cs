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
        private readonly SettingService _settingService;
        public FacebookController(DepartmentService departmentService, UserService userService,
            JobtitleService jobtitleService, PositionService positionService, FacebookService facebookService,SettingService settingService)
        {
            _departmentService = departmentService;
            _userService = userService;
            _jobtitleService = jobtitleService;
            _positionService = positionService;
            _facebookService = facebookService;
            _settingService = settingService;
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
                var PageID = _settingService.Get("setting.facebook.page_id");
                var Token = _settingService.Get("setting.facebook.page_token");
				object result;

                bool hasImages = model.ImageUrls != null && model.ImageUrls.Any();
                bool hasVideos = model.Videos != null && model.Videos.Any();

                if (!hasImages && !hasVideos)
                {
                    result = await _facebookService.PostTextAsync(
                        PageID,
                        model.Messgase,
                        Token
                    );
                }
                else if (hasVideos)
                {
                    result = await _facebookService.PostVideoAsync(
                        PageID,
                        model.Videos,
                        model.Messgase,
                        Token
                    );
                }
                else
                {
                    result = await _facebookService.PostImagesAsync(
                        PageID,
                        model.ImageUrls,
                        model.Messgase,
                        Token
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
