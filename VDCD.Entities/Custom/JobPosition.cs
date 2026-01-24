using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class JobPosition
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Slug { get; set; }

        // Lưu HTML từ CKEditor
        public string? Description { get; set; }

        public string? Location { get; set; }

        // Full-time / Part-time
        public string? EmploymentType { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}
