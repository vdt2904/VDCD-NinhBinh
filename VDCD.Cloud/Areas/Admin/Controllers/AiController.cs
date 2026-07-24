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
        private readonly IAiService _aiService;
        public AiController(
            IAiPostService service, IAiService aiService)
        {
            _service = service;
            _aiService = aiService;
		}

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GeneratePostRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.Topic))
                return BadRequest("Topic is required");

            try
            {
                // Pass attachments to service
                var post = await _service.GenerateAndSave(req.Topic, req.FbAttachmentsList);

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

        [HttpPost("AiGenrerate")]
        public async Task<IActionResult> AiGenrerate([FromBody] string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return BadRequest("Topic is required");
            try
            {
                var content = await _aiService.GeneratePost(topic);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Generated successfully",
                    Data = content
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
		}
	}
}