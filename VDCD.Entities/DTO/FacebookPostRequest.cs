using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class FacebookPostRequest    
    {
        public int? Id { get; set; }
        public string? Messgase { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? Videos { get; set; }
        public string? title { get; set; }
        public DateTime? scheduleDate { get; set; }
        public string? type { get; set; }
    }
}
