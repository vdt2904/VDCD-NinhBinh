using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class JobApplication
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        // Lưu path file CV
        public string? CVFile { get; set; }

        public string? Message { get; set; }

        // New / Reviewed
        public string? Status { get; set; }
        public string? Review { get; set; }

        public DateTime? ApplyDate { get; set; }

    }
}
