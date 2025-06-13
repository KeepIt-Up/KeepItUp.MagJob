using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class OrganizationConfiguration : BaseEntityConfiguration<Organization>
{
    public override void Configure(EntityTypeBuilder<Organization> builder)
    {
        base.Configure(builder);

        #region Properties
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(o => o.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.Property(o => o.OwnerId)
            .IsRequired();

        builder.Property(o => o.IsActive)
            .IsRequired();
        #endregion

        #region Relationships
        builder.HasMany(o => o.Members)
            .WithOne()
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Roles)
            .WithOne()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Invitations)
            .WithOne()
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region Indexes
        builder.HasIndex(o => o.Name);

        // Index for quick search by OwnerId
        builder.HasIndex(o => o.OwnerId);

        // Index for filtering by IsActive
        builder.HasIndex(o => o.IsActive);

        // Index supporting sorting by Id DESC, which is often used in pagination
        builder.HasIndex(o => o.Id).IsDescending();
        #endregion
    }

    protected override string GetTableName() => DataSchemaConstants.ORGANIZATIONS_TABLE;
}
