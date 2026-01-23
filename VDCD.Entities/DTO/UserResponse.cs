using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.DTO
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Profile { get; set; }
        public bool? IsShow { get; set; }
        public bool? IsActive { get; set; }
        public string? DepartmentName { get; set; }
    }

}
