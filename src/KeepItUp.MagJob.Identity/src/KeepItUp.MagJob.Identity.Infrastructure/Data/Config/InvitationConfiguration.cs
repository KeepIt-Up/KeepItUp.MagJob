using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class InvitationConfiguration : BaseEntityConfiguration<Invitation>
{
    protected override string GetTableName() => DataSchemaConstants.INVITATIONS_TABLE;

    public override void Configure(EntityTypeBuilder<Invitation> builder)
    {
        base.Configure(builder);

        #region Properties

        builder.Property(i => i.OrganizationId)
            .IsRequired();

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_EMAIL_LENGTH);

        builder.Property(i => i.Token)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_TOKEN_LENGTH);

        builder.Property(i => i.RoleId)
            .IsRequired();

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(i => i.ExpiresAt)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(i => i.Token).IsUnique();

        builder.HasIndex(i => new { i.Email, i.OrganizationId, i.Status });

        #endregion
    }
}
