using KeepItUp.MagJob.Identity.Core.ContributorAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

public class ContributorConfiguration : BaseEntityConfiguration<Contributor>
{
  public override void Configure(EntityTypeBuilder<Contributor> builder)
  {
    base.Configure(builder);

    builder.Property(p => p.Name)
        .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
        .IsRequired();

    builder.OwnsOne(builder => builder.PhoneNumber);

    builder.Property(x => x.Status)
      .HasConversion(
          x => x.Value,
          x => ContributorStatus.FromValue(x));
  }

  protected override string GetTableName() => DataSchemaConstants.CONTRIBUTOR_TABLE;
}
