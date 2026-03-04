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
    public class ActivityLogMap : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("activitylogs");
            builder.HasKey(x => x.ActivityLogId);
        }
    }
}
