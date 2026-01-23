using VDCD.Entities.Custom;
using VDCD.Entities.DTO;

namespace VDCD.Models
{
    public class OrganizationalModelView
    {
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public List<UserResponse>? Users { get; set; }

    }
}
