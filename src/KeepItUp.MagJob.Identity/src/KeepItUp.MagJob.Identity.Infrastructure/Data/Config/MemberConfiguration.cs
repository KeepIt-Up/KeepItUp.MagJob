using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class MemberConfiguration : BaseEntityConfiguration<Member>
{
    protected override string GetTableName() => DataSchemaConstants.MEMBERS_TABLE;

    public override void Configure(EntityTypeBuilder<Member> builder)
    {
        base.Configure(builder);

        #region Properties
        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.OrganizationId)
            .IsRequired();

        builder.Property(m => m.JoinedAt)
            .IsRequired();
        #endregion

        #region Relationships
        // Many-to-many relationship with Role
        builder.HasMany(m => m.Roles)
            .WithMany(r => r.Members)
            .UsingEntity<Dictionary<string, object>>(
                DataSchemaConstants.MEMBER_ROLES_TABLE,
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<Member>().WithMany().HasForeignKey("MemberId"),
                j =>
                {
                    j.HasKey("MemberId", "RoleId");
                    j.ToTable(DataSchemaConstants.MEMBER_ROLES_TABLE, DataSchemaConstants.IDENTITY_SCHEMA);

                    // Add indexes for the join table
                    j.HasIndex("MemberId");
                    j.HasIndex("RoleId");
                });
        #endregion

        #region Indexes
        builder.HasIndex(m => new { m.UserId, m.OrganizationId }).IsUnique();

        // Index for quick search by UserId
        builder.HasIndex(m => m.UserId);

        // Index for quick search by OrganizationId
        builder.HasIndex(m => m.OrganizationId);
        #endregion
    }
}
