using KeepItUp.MagJob.Identity.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

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

        // Konfiguracja Value Object UserProfile
        builder.OwnsOne(u => u.Profile, profile =>
        {
            profile.Property(p => p.PhoneNumber)
                .HasMaxLength(DataSchemaConstants.DEFAULT_PHONE_NUMBER_LENGTH);

            profile.Property(p => p.Address)
                .HasMaxLength(DataSchemaConstants.DEFAULT_ADDRESS_LENGTH);

            profile.Property(p => p.ProfileImage)
                .HasMaxLength(DataSchemaConstants.DEFAULT_PROFILE_IMAGE_LENGTH);
            
            // Dodanie właściwości dyskryminatora, aby EF Core mógł określić, czy encja istnieje
            profile.Property<bool>("IsProfileCreated")
                .HasDefaultValue(true);
        });

        // Indeksy
        builder.HasIndex(u => u.ExternalId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }

    protected override string GetTableName() => DataSchemaConstants.USERS_TABLE;
} 
