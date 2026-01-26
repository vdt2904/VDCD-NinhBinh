using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VDCD.Business.Service;
using VDCD.Entities.Custom;

namespace VDCD.Controllers
{
    public class CareersController : BaseController
    {
        private readonly SeoMetaService _seoMetaService;
        private readonly SettingService _settingService;
        private readonly CategoryService _categoryService;
        private readonly ProjectService _projectService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly JobPositionService _jobPositionService;
        public CareersController(SeoMetaService seoMetaService, SettingService settingService, CategoryService categoryService,
            ProjectService projectService, JobApplicationService jobApplicationService, JobPositionService jobPositionService) : base(seoMetaService)
        {
            _seoMetaService = seoMetaService;
            _settingService = settingService;
            _categoryService = categoryService;
            _projectService = projectService;
            _jobApplicationService = jobApplicationService;
            _jobPositionService = jobPositionService;
        }
        public IActionResult Index()
        {
            ApplySeo("tuyen-dung");
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            var lst = _jobPositionService.GetAll();
            return View(lst);
        }
        public IActionResult Details(string slug)
        {
            var job = _jobPositionService.GetBySlug(slug);
            if (job == null)
            {
                return NotFound();
            }
            ApplySeo("jobposition:"+job.Slug);
            var lstSetting = _settingService.GetAll();

            // Biến toàn bộ list thành Dictionary để tra cứu theo Key
            // ToDictionary giúp truy cập giá trị cực nhanh, không ảnh hưởng hiệu suất khi dữ liệu lớn
            var settingsDic = lstSetting.ToDictionary(x => x.SettingKey, x => x.Value);
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Settings = settingsDic;
            return View(job);
        }
        public async Task<IActionResult> PostApply([FromForm] JobApplication app, IFormFile CVFileUpdate)
        {
            // 1. Kiểm tra tính hợp lệ cơ bản
            if (CVFileUpdate == null || CVFileUpdate.Length == 0)
            {
                return BadRequest(new { success = false, message = "Vui lòng đính kèm tệp CV của bạn." });
            }

            // 2. Kiểm tra định dạng file (Chỉ cho phép PDF, DOCX)
            var allowedExtensions = new[] { ".pdf", ".docx", ".doc" };
            var extension = Path.GetExtension(CVFileUpdate.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { success = false, message = "Định dạng file không hỗ trợ. Vui lòng gửi PDF hoặc Word." });
            }

            // 3. Kiểm tra dung lượng (Ví dụ tối đa 5MB)
            if (CVFileUpdate.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { success = false, message = "Dung lượng file quá lớn (Tối đa 5MB)." });
            }

            try
            {
                // 4. Xử lý lưu File vật lý
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                // Tạo tên file duy nhất để tránh trùng lặp
                string uniqueFileName = $"CV_{Guid.NewGuid()}_{DateTime.Now:yyyyMMdd}{extension}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await CVFileUpdate.CopyToAsync(fileStream);
                }

                // 5. Gán giá trị bổ sung cho Model
                app.CVFile = "/uploads/cvs/" + uniqueFileName;
                app.ApplyDate = DateTime.Now;
                app.Status = "New"; // Trạng thái mặc định khi mới nộp

                // 6. Lưu vào Database
                await _jobApplicationService.Save(app);


                return Ok(new { success = true, message = "Hồ sơ của bạn đã được gửi thành công!" });
            }
            catch (Exception ex)
            {
                // Log lỗi tại đây (ví dụ: ILogger)
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
