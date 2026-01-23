using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class Department
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public string? DepartmentName { get; set; }
        public string? DepartmentExt { get; set; }
        public string? DepartmentPath { get; set; }

        public bool IsActive { get; set; } = false;

        public int? CreateId { get; set; }
        public DateTime? DateCreate { get; set; }

        public int? ModifyId { get; set; }
        public DateTime? DateModify { get; set; }

        public string? Emails { get; set; }
    }

}
