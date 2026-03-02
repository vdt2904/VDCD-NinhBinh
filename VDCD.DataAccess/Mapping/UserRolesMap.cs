using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VDCD.Entities.Custom;

namespace VDCD.DataAccess.Mapping;

public class UserRolesMap : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.RoleName).HasColumnName("role_name").HasMaxLength(50);
        builder.Property(x => x.CreateAt).HasColumnName("created_at");
        builder.Property(x => x.UpdateAt).HasColumnName("updated_at");

        builder.HasIndex(x => x.UserId).IsUnique();
    }
}
