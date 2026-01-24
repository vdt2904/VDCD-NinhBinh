using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.IO;
using MailKit.Net.Imap;
using static System.Environment;
using MailKit;
using SpecialFolder = MailKit.SpecialFolder;

namespace VDCD.Business.Service
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ContactMessageService _contactService;
        private readonly SettingService _settingService;    

        public EmailService(IConfiguration config, ContactMessageService contactService, SettingService settingService)
        {
            _config = config;
            _contactService = contactService;
            _settingService = settingService;
        }
        public void SendReplyEmailWithAttachments(int id, string toEmail, string subject, string htmlContent, List<string> filePaths)
        {
            // 1. Lấy thông tin khách hàng để replace placeholder
            var contact = _contactService.GetById(id);
            string customerName = contact?.Name ?? "Quý khách";

            string finalBody = htmlContent
                .Replace("{name}", customerName)
                .Replace("{time}", DateTime.Now.ToString("HH:mm dd/MM/yyyy"));

            // 2. LẤY CẤU HÌNH TỪ DATABASE (Thay vì AppSettings)
            var lstSetting = _settingService.GetAll();

            // Tìm các giá trị dựa trên SettingKey bạn đã cung cấp
            string smtpServer = lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.smtp_server")?.Value;
            int port = int.Parse(lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.port")?.Value ?? "587");
            string senderName = lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.sender_name")?.Value ?? "VDCD";
            string senderEmail = lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.sender_email")?.Value;
            string appPassword = lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.app_password")?.Value;
            bool enableSsl = bool.Parse(lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.enable_ssl")?.Value ?? "True");
            string imapServer = lstSetting.FirstOrDefault(x => x.SettingKey == "setting.email.imap_server")?.Value
                        ?? smtpServer;
            int imapPort = 993;

            // 3. Khởi tạo Email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = finalBody };

            // Đính kèm file vật lý
            if (filePaths != null && filePaths.Count > 0)
            {
                foreach (var path in filePaths)
                {
                    if (File.Exists(path))
                    {
                        bodyBuilder.Attachments.Add(path);
                    }
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // 4. Gửi Mail qua SMTP
            using (var client = new SmtpClient())
            {
                try
                {
                    // Bizfly thường dùng StartTls với cổng 587
                    var secureOption = enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

                    client.Connect(smtpServer, port, secureOption);
                    client.Authenticate(senderEmail, appPassword);
                    client.Send(message);
                    client.Disconnect(true);

                    // 5. Xóa file tạm
                    DeleteTempFiles(filePaths);
                }
                catch (Exception)
                {
                    throw; // Hangfire sẽ nhận lỗi này để thực hiện Retry
                }
            }


        }
        private void DeleteTempFiles(List<string> filePaths)
        {
            if (filePaths == null) return;
            foreach (var path in filePaths)
            {
                if (File.Exists(path)) File.Delete(path);
            }

            // Thử xóa thư mục cha nếu trống
            var folder = Path.GetDirectoryName(filePaths.FirstOrDefault());
            if (folder != null && Directory.Exists(folder) && !Directory.EnumerateFileSystemEntries(folder).Any())
            {
                Directory.Delete(folder);
            }
        }
    }
}
