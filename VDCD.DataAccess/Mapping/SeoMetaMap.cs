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
    public class SeoMetaMap : IEntityTypeConfiguration<SeoMeta>
    {
        public void Configure(EntityTypeBuilder<SeoMeta> builder)
        {
            builder.ToTable("seo_meta");
            builder.HasKey(x => x.Id);
        }
    }
}
