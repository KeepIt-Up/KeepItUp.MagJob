using KeepItUp.MagJob.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.ContributorAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o aktualizacji statusu kontrybutora.
/// </summary>
public class ContributorStatusUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator kontrybutora.
    /// </summary>
    public Guid ContributorId { get; }

    /// <summary>
    /// Nazwa kontrybutora.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Status kontrybutora.
    /// </summary>
    public ContributorStatus Status { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie ContributorStatusUpdatedEvent.
    /// </summary>
    /// <param name="contributorId">Identyfikator kontrybutora.</param>
    /// <param name="name">Nazwa kontrybutora.</param>
    /// <param name="status">Status kontrybutora.</param>
    public ContributorStatusUpdatedEvent(Guid contributorId, string name, ContributorStatus status)
    {
        ContributorId = contributorId;
        Name = name;
        Status = status;
    }
}
