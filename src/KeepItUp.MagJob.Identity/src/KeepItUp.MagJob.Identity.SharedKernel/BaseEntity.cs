using KeepItUp.MagJob.Identity.SharedKernel.Core;

namespace KeepItUp.MagJob.Identity.SharedKernel;

/// <summary>
/// Base class for all entities in the system.
/// </summary>
public abstract class BaseEntity : EntityBase<Guid>
{

    /// <summary>
    /// Entity version for optimistic concurrency.
    /// </summary>
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    /// <summary>
    /// Updates the last modification date of the entity.
    /// </summary>
    protected void Update()
    {
        UpdatedAt = DateTime.UtcNow;
        RowVersion = Guid.NewGuid().ToByteArray();
    }

    /// <summary>
    /// Base constructor for all entities.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        RowVersion = Guid.NewGuid().ToByteArray();
    }

    /// <summary>
    /// Registers a domain event and updates the modification date of the entity.
    /// </summary>
    /// <param name="domainEvent">Domain event to register.</param>
    protected void RegisterDomainEventAndUpdate(DomainEventBase domainEvent)
    {
        RegisterDomainEvent(domainEvent);
        Update();
    }
}
