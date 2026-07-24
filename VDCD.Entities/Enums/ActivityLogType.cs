using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VDCD.Entities.Enums
{
    public enum ActivityLogType : byte
    {
        [Description("Đăng nhập")]
        Login = 1,

        [Description("Đăng xuất")]
        Logout = 2,

        [Description("Quản lý bài viết")]
        Post = 10,

        [Description("Người dùng")]
        User = 12
    }
}
