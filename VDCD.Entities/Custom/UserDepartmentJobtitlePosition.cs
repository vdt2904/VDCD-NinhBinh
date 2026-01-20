using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class UserDepartmentJobtitlePosition
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobtitleId { get; set; }
        public int? PositionId { get; set; }

        public bool IsMain { get; set; } = false;
    }
}
