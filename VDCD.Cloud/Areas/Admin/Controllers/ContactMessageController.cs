using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VDCD.Business.Service;
using VDCD.Entities.Security;

namespace VDCD.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth", Roles = AdminRoles.ContentAccess)]
    public class ContactMessageController : Controller
    {
        private readonly ContactMessageService _contactService;
        public ContactMessageController(ContactMessageService contactMessage)
        {
            _contactService = contactMessage;
        }
        public IActionResult Index(int page = 1, string searchTerm = "", string filterType = "all")
        {
            int pageSize = 10;
            var query = _contactService.GetContactMessages().AsQueryable();

            // 1. Lọc theo trạng thái
            if (filterType == "unread")
            {
                query = query.Where(x => !x.IsRead);
            }

            // 2. Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(x =>
                    (x.Name != null && x.Name.ToLower().Contains(searchTerm)) ||
                    (x.Email != null && x.Email.ToLower().Contains(searchTerm)) ||
                    (x.Subject != null && x.Subject.ToLower().Contains(searchTerm)) ||
                    (x.Content != null && x.Content.ToLower().Contains(searchTerm))
                );
            }

            var data = query.OrderByDescending(x => x.CreatedAt)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ContactListPartial", data);
            }

            return View(data);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
             _contactService.DeleteContact(id); // Giả định hàm xóa của bạn
            return Json(new { success = true });
        }
        public ActionResult IsRead(int id) {
            try
            {
                _contactService.IsRead(id); 
                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message= ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ReplyMessage(int id, string content, List<IFormFile> attachments)
        {
            try
            {
                // 1. Lấy tin nhắn gốc (Dùng phương thức đồng bộ của Service bạn)
                var message = _contactService.GetById(id);
                if (message == null)
                    return Json(new { success = false, message = "Không tìm thấy tin nhắn." });

                // 2. Cập nhật DB
                message.ReplyContent = content;
                message.RepliedAt = DateTime.Now;
                _contactService.Update(message);

                // 3. Lưu file vật lý đồng bộ
                List<string> savedFilePaths = new List<string>();
                if (attachments != null && attachments.Count > 0)
                {
                    var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "temp_replies", id.ToString());
                    if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

                    foreach (var file in attachments)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(tempFolder, fileName);

                        // Dùng FileStream đồng bộ
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        savedFilePaths.Add(filePath);
                    }
                }

                // 4. Đẩy vào Hangfire (Hangfire luôn hỗ trợ gọi hàm đồng bộ)
                BackgroundJob.Enqueue<EmailService>(x =>
                    x.SendReplyEmailWithAttachments(message.Id, message.Email, "Phản hồi: " + message.Subject, content, savedFilePaths));

                return Json(new { success = true, message = "Đã xếp hàng gửi mail!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
