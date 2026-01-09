using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class FilePickerItemDto
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public bool IsFolder { get; set; }
        public string? ContentType { get; set; }
    }

}
