using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VDCD.Business.Infrastructure;
using VDCD.Business.Service;
using VDCD.DataAccess;
using VDCD.Entities.Custom;
using VDCD.Entities.DTO;
using VDCD.Models;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IAiPostService _service;

        public AiController(
            IAiPostService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GeneratePostRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.Topic))
                return BadRequest("Topic is required");

            try
            {
                var post = await _service.GenerateAndSave(req.Topic);

                return Ok(new ApiResponse<FbPost>
                {
                    Success = true,
                    Message = "Generated and saved successfully",
                    Data = post
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpPost("post-now/{id}")]
        //public async Task<IActionResult> PostNow(int id)
        //{
        //    var post = await _db.FbPosts.FindAsync(id);

        //    if (post == null)
        //        return NotFound();

        //    var fbId = await _fb.Post(post.Content);

        //    post.Status = "Posted";
        //    post.FacebookPostId = fbId;

        //    await _db.SaveChangesAsync();

        //    return Ok(new ApiResponse<object>
        //    {
        //        Success = true,
        //        Message = "Post published successfully",
        //        Data = new { FacebookPostId = fbId }
        //    });
        //}
    }
}