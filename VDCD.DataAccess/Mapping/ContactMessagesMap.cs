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
    public class ContactMessagesMap : IEntityTypeConfiguration<ContactMessages>
    {
        public void Configure(EntityTypeBuilder<ContactMessages> builder)
        {
            builder.ToTable("contact_messages");
            builder.HasKey(x => x.Id);
        }
    }
}
