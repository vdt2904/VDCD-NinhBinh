using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class FbPost
    {
        public int Id { get; set; }
        public string? Topic { get; set; }
        public string? Content { get; set; }
        public string Status { get; set; } = "Draft";
        public DateTime? ScheduledAt { get; set; }
        public string? FacebookPostId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // New: serialized attachments (JSON array of urls / identifiers)
        public string? Files { get; set; }
    }
}
