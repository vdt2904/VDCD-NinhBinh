using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class ActivityLog
    {
        [Key]
        public int ActivityLogId { get; set; }

        public byte ActivityLogType { get; set; }   // tinyint

        [MaxLength(15)]
        public string? Ip { get; set; }

        public int? UserId { get; set; }

        [MaxLength(64)]
        public string? UserName { get; set; }

        [Column(TypeName = "text")]
        public string? Content { get; set; }

        public DateTime CreatedOnDate { get; set; }
    }
}
