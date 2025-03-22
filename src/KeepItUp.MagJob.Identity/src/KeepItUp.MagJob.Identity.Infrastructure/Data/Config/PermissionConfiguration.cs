using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(DataSchemaConstants.PERMISSIONS_TABLE, DataSchemaConstants.IDENTITY_SCHEMA);

        builder.HasKey(p => p.Name);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(p => p.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        // Dodanie standardowych uprawnień
        var standardPermissions = Permission.StandardPermissions.GetAll();
        builder.HasData(standardPermissions);
    }
} 
