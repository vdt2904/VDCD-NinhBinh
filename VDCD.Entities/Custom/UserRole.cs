using VDCD.Entities.Security;

namespace VDCD.Entities.Custom;

public class UserRole
{
    public int Id { get; set; }
	//	public int RoleId { get; set; }
    public int UserId { get; set; }
    public string RoleName { get; set; } = AdminRoles.Viewer;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime? UpdateAt { get; set; }
}
