using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class FacebookPost
    {
        public int Id { get; set; }

        public string? PageId { get; set; }      // ID fanpage
        public string? Message { get; set; }     // Nội dung text

        public string? ImageUrls { get; set; }   // JSON list
        public string? VideoUrl { get; set; }    // 1 video

        public DateTime CreatedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }

        public bool IsPosted { get; set; }
        public DateTime? PostedDate { get; set; }

        public string? FacebookPostId { get; set; }
        public string? ErrorMessage { get; set; }
    }

}
