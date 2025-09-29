using KeepItUp.MagJob.Identity.SharedKernel;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

/// <summary>
/// Base entity configuration for all entities inheriting from BaseEntity.
/// </summary>
/// <typeparam name="TEntity">Typ encji.</typeparam>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    protected abstract string GetTableName();

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(GetTableName(), DataSchemaConstants.IDENTITY_SCHEMA);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);
    }
}
