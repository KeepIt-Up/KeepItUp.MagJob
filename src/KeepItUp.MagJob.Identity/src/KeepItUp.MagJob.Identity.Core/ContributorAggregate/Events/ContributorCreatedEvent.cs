namespace KeepItUp.MagJob.Identity.Core.ContributorAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o utworzeniu nowego kontrybutora.
/// </summary>
public class ContributorCreatedEvent : DomainEventBase
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
    /// Tworzy nowe zdarzenie ContributorCreatedEvent.
    /// </summary>
    /// <param name="contributorId">Identyfikator kontrybutora.</param>
    /// <param name="name">Nazwa kontrybutora.</param>
    public ContributorCreatedEvent(Guid contributorId, string name)
    {
        ContributorId = contributorId;
        Name = name;
    }
}
