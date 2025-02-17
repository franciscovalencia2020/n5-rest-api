using Data.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.HasKey(up => up.Id);
            builder.HasOne(up => up.User)
                   .WithMany()
                   .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.Permission)
                   .WithMany()
                   .HasForeignKey(up => up.PermissionId);

            builder.HasIndex(up => new { up.UserId, up.PermissionId }).IsUnique();
        }
    }
}
