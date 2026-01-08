using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VDCD.Entities;
using VDCD.Entities.Custom;

namespace VDCD.DataAccess.Mapping;

public class UsersMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.UserId);
    }
}
