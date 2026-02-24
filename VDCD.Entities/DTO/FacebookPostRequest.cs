using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class FacebookPostRequest
    {
        public string? PageId { get; set; }
        public string? Messgase { get; set; }
        public string? Token { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? Videos { get; set; }
    }
}
