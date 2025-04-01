namespace KeepItUp.MagJob.Identity.SharedKernel;

/// <summary>
/// Klasa bazowa dla wszystkich encji w systemie.
/// </summary>
public abstract class BaseEntity : EntityBase<Guid>
{
    /// <summary>
    /// Data utworzenia encji.
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Data ostatniej aktualizacji encji.
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// Wersja encji do optymistycznej współbieżności.
    /// </summary>
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    /// <summary>
    /// Aktualizuje datę ostatniej modyfikacji encji.
    /// </summary>
    protected void Update()
    {
        UpdatedAt = DateTime.UtcNow;
        RowVersion = Guid.NewGuid().ToByteArray();
    }

    /// <summary>
    /// Konstruktor bazowy dla wszystkich encji.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        RowVersion = Guid.NewGuid().ToByteArray();
    }

    /// <summary>
    /// Rejestruje zdarzenie domenowe i aktualizuje datę modyfikacji encji.
    /// </summary>
    /// <param name="domainEvent">Zdarzenie domenowe do zarejestrowania.</param>
    protected void RegisterDomainEventAndUpdate(DomainEventBase domainEvent)
    {
        RegisterDomainEvent(domainEvent);
        Update();
    }
}
