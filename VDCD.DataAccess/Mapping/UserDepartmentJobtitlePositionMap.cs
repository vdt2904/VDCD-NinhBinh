using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDCD.Entities.Custom;

namespace VDCD.DataAccess.Mapping
{
    public class UserDepartmentJobtitlePositionMap : IEntityTypeConfiguration<UserDepartmentJobtitlePosition>
    {
        public void Configure(EntityTypeBuilder<UserDepartmentJobtitlePosition> builder)
        {
            builder.ToTable("user_department_jobtitle_position");
            builder.HasKey(x => x.Id);
        }
    }

}
