using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class MemberConfiguration : BaseEntityConfiguration<Member>
{
    public override void Configure(EntityTypeBuilder<Member> builder)
    {
        base.Configure(builder);

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.OrganizationId)
            .IsRequired();

        builder.Property(m => m.JoinedAt)
            .IsRequired();

        // Konfiguracja kolekcji RoleIds
        builder.Property<List<Guid>>("_roleIds")
            .HasColumnName("RoleIds")
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => Guid.Parse(id))
                    .ToList(),
                new ValueComparer<List<Guid>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        // Relacja wiele-do-wielu z Role
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
                });

        // Indeksy
        builder.HasIndex(m => new { m.UserId, m.OrganizationId }).IsUnique();
    }

    protected override string GetTableName() => DataSchemaConstants.MEMBERS_TABLE;
}
