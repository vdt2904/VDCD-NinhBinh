using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Text.Json;
using VDCD.Business.Service;
using VDCD.Entities.Custom;
using VDCD.Entities.DTO;
using VDCD.Entities.Security;
using VDCD.Helper;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.ContentAccess)]
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
                    result = await _facebookService.PostTextAsync((int)model.Id,
                        PageID,
                        model.Messgase,
                        Token
                    );
                }
                else if (hasImages && hasVideos)
                {
                    result = await _facebookService.PostVideoWithImagesAsync((int)model.Id,PageID, model.Videos,model.ImageUrls,model.Messgase,Token);
                }
                else if (hasVideos)
                {
                    result = await _facebookService.PostVideoAsync(
						(int)model.Id,

						PageID,
                        model.Videos,
                        model.Messgase,
                        Token
                    );
                }
                else
                {
                    result = await _facebookService.PostImagesAsync(
						(int)model.Id,

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
        public async Task<IActionResult> FaceBookPosts([FromBody] FacebookPostRequest model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Model is null");
                var fbp = new FacebookPost();
                var PageID = _settingService.Get("setting.facebook.page_id");
                var Token = _settingService.Get("setting.facebook.page_token");
                fbp.VideoUrl = model.Videos;
                if(model.ImageUrls == null)
                {
                    fbp.ImageUrls = null;
                }
                else
                {

                fbp.ImageUrls = JsonSerializer.Serialize(model.ImageUrls);
                }
                fbp.Status = 1;
                fbp.Message = model.Messgase;
                fbp.ScheduledDate = (DateTime)model.scheduleDate;
                fbp.Title = model.title;
                fbp.TypePost = model.type;
                fbp.Id = (int)model.Id;
                fbp.PageId = PageID;
                fbp.IsPosted = false;
                fbp.CreatedDate = DateTime.Now;
               var username = Helper.Helper.CurrentUser(HttpContext);
                var user = _userService.GetByUsername(username);
                fbp.UserCreateId = user.UserId;
                _facebookService.save(fbp);
                return Ok(new {success = true});
            }
            catch (Exception ex)
            {
                return Ok(new {success = false , message = ex.Message});
            }
            
        }
        public async Task<IActionResult> ScheculePostFacebook(int id)
        {
            try
            {
                var fbp = _facebookService.Get(id);
                fbp.Status = 2;
                var username = Helper.Helper.CurrentUser(HttpContext);
                var user = _userService.GetByUsername(username);
                fbp.UserReviewerId = user.UserId;

                bool hasImages = fbp.ImageUrls != null && fbp.ImageUrls.Any();
                bool hasVideos = fbp.VideoUrl != null && fbp.VideoUrl.Any();
                var PageID = _settingService.Get("setting.facebook.page_id");
                var Token = _settingService.Get("setting.facebook.page_token");
                var imageList = string.IsNullOrEmpty(fbp.ImageUrls)
? new List<string>()
: JsonSerializer.Deserialize<List<string>>(fbp.ImageUrls);
                string jobId = null;
                if (!hasImages && !hasVideos)
                {
                    jobId = BackgroundJob.Schedule<FacebookService>(x =>
                        x.PostTextAsync(id,PageID, fbp.Message, Token),
                        fbp.ScheduledDate.Value);
                }
                else if (hasImages && hasVideos)
                {
                    jobId = BackgroundJob.Schedule<FacebookService>(x =>
                        x.PostVideoWithImagesAsync(id, PageID, fbp.VideoUrl, imageList, fbp.Message, Token),
                        fbp.ScheduledDate.Value);
                }
                else if (hasVideos)
                {
                    jobId = BackgroundJob.Schedule<FacebookService>(x =>
                        x.PostVideoAsync(id, PageID, fbp.VideoUrl, fbp.Message, Token),
                        fbp.ScheduledDate.Value);
                }
                else
                {
                    jobId = BackgroundJob.Schedule<FacebookService>(x =>
                        x.PostImagesAsync(id,PageID, imageList, fbp.Message, Token),
                        fbp.ScheduledDate.Value);
                }
                fbp.FacebookPostId = jobId;
                _facebookService.save(fbp);
                return Ok(new {success = true});

            }catch (Exception ex) {
                return Ok(new { success = false, message = ex.Message });
            }
        }
		public async Task<IActionResult> DeleteScheculePostFaceBooke(int id, int status)
		{
			try
			{
				var fbp = _facebookService.Get(id);
				if (fbp == null)
				{
					return Ok(new { success = false, message = "Không tìm thấy bài viết" });
				}

				bool jobDeleted = false;

				// Trường hợp có lịch
				if (!string.IsNullOrEmpty(fbp.FacebookPostId))
				{
					jobDeleted = BackgroundJob.Delete(fbp.FacebookPostId);

					// Clear jobId dù delete thành công hay không
					fbp.FacebookPostId = null;
				}

				// Nếu status = 0 → xóa luôn record
				if (status == 0)
				{
					_facebookService.Delete(id);
				}
				else
				{
					fbp.Status = status;
					_facebookService.save(fbp);
				}

				return Ok(new
				{
					success = true,
					jobDeleted = jobDeleted // để bạn biết job có xóa được không
				});
			}
			catch (Exception ex)
			{
				return Ok(new { success = false, message = ex.Message });
			}
		}
		public async Task<IActionResult> GetAll(int page = 1, int pagesize = 5)
		{
			try
			{
				if (page <= 0) page = 1;
				if (pagesize <= 0) pagesize = 5;

				var allData = _facebookService.GetAll();

				var total = allData.Count;
				var totalPages = (int)Math.Ceiling((double)total / pagesize);

				var data = allData
					.OrderByDescending(x => x.Id) // hoặc ScheduledDate tùy bạn
					.Skip((page - 1) * pagesize)
					.Take(pagesize)
					.ToList();

				return Ok(new
				{
					success = true,
					page,
					pagesize,
					total,
					totalPages,
					data
				});
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					success = false,
					message = ex.Message
				});
			}
		}
	}
}
