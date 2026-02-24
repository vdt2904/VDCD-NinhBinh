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
    public class FacebookTokensMap : IEntityTypeConfiguration<FacebookToken>
    {
        public void Configure(EntityTypeBuilder<FacebookToken> builder)
        {
            builder.ToTable("FacebookTokens");
            builder.HasKey(x => x.Id);
        }
    }
}
