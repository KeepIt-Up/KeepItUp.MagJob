namespace KeepItUp.MagJob.Identity.Core.ContributorAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o aktualizacji kontrybutora.
/// </summary>
public class ContributorUpdatedEvent : DomainEventBase
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
    /// Tworzy nowe zdarzenie ContributorUpdatedEvent.
    /// </summary>
    /// <param name="contributorId">Identyfikator kontrybutora.</param>
    /// <param name="name">Nazwa kontrybutora.</param>
    public ContributorUpdatedEvent(Guid contributorId, string name)
    {
        ContributorId = contributorId;
        Name = name;
    }
}
