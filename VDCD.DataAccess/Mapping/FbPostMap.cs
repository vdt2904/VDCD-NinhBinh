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
    public class FbPostMap : IEntityTypeConfiguration<FbPost>
    {
        public void Configure(EntityTypeBuilder<FbPost> builder)
        {
            builder.ToTable("fb_posts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Topic).HasMaxLength(255);

            builder.Property(x => x.Status)
                   .HasMaxLength(20)
                   .HasDefaultValue("Draft");
        }
    }
}
