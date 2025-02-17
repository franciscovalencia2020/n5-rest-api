using Data.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.PermissionType)
                   .WithMany()
                   .HasForeignKey(p => p.PermissionTypeId);
        }
    }
}
