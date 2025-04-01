using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class InvitationConfiguration : BaseEntityConfiguration<Invitation>
{
    public override void Configure(EntityTypeBuilder<Invitation> builder)
    {
        base.Configure(builder);

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

        // Indeksy
        builder.HasIndex(i => i.Token).IsUnique();
        builder.HasIndex(i => new { i.Email, i.OrganizationId, i.Status });
    }

    protected override string GetTableName() => DataSchemaConstants.INVITATIONS_TABLE;
}
