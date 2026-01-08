using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
    public class User
    {
        public int UserId { get; set; }                // int(0) => int
        public string? UserName { get; set; }           // varchar(255)
        public string? HashPassword { get; set; }       // varchar(500)
        public string? FullName { get; set; }           // varchar(255)
        public string? Mail { get; set; }               // varchar(500)
        public string? Phone { get; set; }              // varchar(11)
        public bool? IsActive { get; set; }             // bit(1) => bool
        public DateTime? CreateAt { get; set; }         // datetime
    }
}
