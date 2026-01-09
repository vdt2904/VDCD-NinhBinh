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
    internal class FIlesMap : IEntityTypeConfiguration<Files>
    {
        public void Configure(EntityTypeBuilder<Files> builder)
        {
            builder.ToTable("Files");
            builder.HasKey(x => x.Id);
        }
    }
}
