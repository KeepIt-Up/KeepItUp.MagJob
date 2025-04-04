using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class OrganizationConfiguration : BaseEntityConfiguration<Organization>
{
    public override void Configure(EntityTypeBuilder<Organization> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(o => o.Description)
            .HasMaxLength(DataSchemaConstants.DEFAULT_DESCRIPTION_LENGTH);

        builder.Property(o => o.OwnerId)
            .IsRequired();

        builder.Property(o => o.IsActive)
            .IsRequired();

        // Relacje
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

        // Indeksy
        builder.HasIndex(o => o.Name);

        // Indeks dla szybkiego wyszukiwania po OwnerId
        builder.HasIndex(o => o.OwnerId);

        // Indeks dla filtrowania po IsActive
        builder.HasIndex(o => o.IsActive);

        // Indeks wspierający sortowanie po Id DESC, które jest często używane w paginacji
        builder.HasIndex(o => o.Id).IsDescending();
    }

    protected override string GetTableName() => DataSchemaConstants.ORGANIZATIONS_TABLE;
}
