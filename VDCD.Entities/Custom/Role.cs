using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDCD.Entities.Custom
{
	public class Role
	{
		public int RoleID { get; set; }
		public string? RoleKey { get; set; }
		public string? RoleName { get; set; }
		public string? Desctiption { get; set; }
		public bool IsActive { get; set; } = true;
	}
}
