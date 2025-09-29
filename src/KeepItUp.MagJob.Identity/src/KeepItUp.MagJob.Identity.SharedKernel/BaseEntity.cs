using KeepItUp.MagJob.Identity.SharedKernel.Core;

namespace KeepItUp.MagJob.Identity.SharedKernel;

/// <summary>
/// Base class for all entities in the system.
/// </summary>
public abstract class BaseEntity : EntityBase<Guid>
{
    /// <summary>
    /// Updates the last modification date of the entity.
    /// </summary>
    protected void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Base constructor for all entities.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
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
