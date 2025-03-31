using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class RoleConfiguration : BaseEntityConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(r => r.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.Property(r => r.Color)
            .HasMaxLength(DataSchemaConstants.DEFAULT_COLOR_LENGTH);

        builder.Property(r => r.OrganizationId)
            .IsRequired();

        // Konfiguracja kolekcji Permissions
        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                DataSchemaConstants.ROLE_PERMISSIONS_TABLE,
                j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                j =>
                {
                    j.HasKey("RoleId", "PermissionId");
                    j.ToTable(DataSchemaConstants.ROLE_PERMISSIONS_TABLE, DataSchemaConstants.IDENTITY_SCHEMA);

                    // Add indexes for join table
                    j.HasIndex("RoleId");
                    j.HasIndex("PermissionId");
                });

        // Indeksy
        builder.HasIndex(r => new { r.Id, r.OrganizationId }).IsUnique();

        // Indeks dla szybkiego wyszukiwania po OrganizationId
        builder.HasIndex(r => r.OrganizationId);

        // Indeks dla wyszukiwania ról po nazwie
        builder.HasIndex(r => r.Name);
    }

    protected override string GetTableName() => DataSchemaConstants.ROLES_TABLE;
}
