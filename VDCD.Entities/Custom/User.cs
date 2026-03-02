using System.ComponentModel.DataAnnotations.Schema;

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
        public string? Profile {  get; set; }
        public bool? IsShow { get; set; } = false;
        public string? Avatar { get; set; }

        [NotMapped]
        public string? RoleName { get; set; }
    }
}
