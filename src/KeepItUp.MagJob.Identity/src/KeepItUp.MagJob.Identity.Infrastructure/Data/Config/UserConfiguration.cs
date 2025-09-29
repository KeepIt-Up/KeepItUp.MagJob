using KeepItUp.MagJob.Identity.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    protected override string GetTableName() => DataSchemaConstants.USERS_TABLE;

    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        #region Properties

        builder.Property(u => u.ExternalId)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_EXTERNAL_ID_LENGTH);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_EMAIL_LENGTH);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(u => u.IsActive)
            .IsRequired();

        #endregion

        #region Relationships

        // Configure the Value Object UserProfile
        builder.OwnsOne(u => u.Profile, profile =>
        {
            profile.Property(p => p.PhoneNumber)
                .HasMaxLength(DataSchemaConstants.DEFAULT_PHONE_NUMBER_LENGTH);

            profile.Property(p => p.Address)
                .HasMaxLength(DataSchemaConstants.DEFAULT_ADDRESS_LENGTH);

            profile.Property(p => p.ProfileImage)
                .HasMaxLength(DataSchemaConstants.DEFAULT_PROFILE_IMAGE_LENGTH);

            // Add the discriminator property to determine if the entity exists
            profile.Property<bool>("IsProfileCreated")
                .HasDefaultValue(true);
        });

        // Configure the collection of Permissions (as a list of strings)
        builder.Property<List<string>>("_permissions")
            .HasColumnName("Permissions")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        // Configure the relationship with Member
        builder.HasMany(u => u.Memberships)
            .WithOne()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Indexes

        builder.HasIndex(u => u.ExternalId).IsUnique();

        builder.HasIndex(u => u.Email).IsUnique();

        #endregion
    }
}
