using KeepItUp.MagJob.SharedKernel;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data.Config;

/// <summary>
/// Bazowa konfiguracja dla wszystkich encji dziedziczących po BaseEntity.
/// </summary>
/// <typeparam name="TEntity">Typ encji.</typeparam>
public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Ustawienie schematu "identity" dla wszystkich encji
        builder.ToTable(GetTableName(), "identity");

        // Konfiguracja klucza głównego
        builder.HasKey(e => e.Id);

        // Konfiguracja pól z BaseEntity
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);
    }

    /// <summary>
    /// Zwraca nazwę tabeli dla encji.
    /// </summary>
    /// <returns>Nazwa tabeli.</returns>
    protected abstract string GetTableName();
}
