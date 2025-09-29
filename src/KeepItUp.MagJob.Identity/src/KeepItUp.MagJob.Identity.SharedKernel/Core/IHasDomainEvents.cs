namespace KeepItUp.MagJob.Identity.SharedKernel.Core;

/// <summary>
/// Interface for entities that support domain events.
/// </summary>
public interface IHasDomainEvents
{
  /// <summary>
  /// Gets the read-only collection of domain events.
  /// </summary>
  IReadOnlyCollection<DomainEventBase> DomainEvents { get; }
}
