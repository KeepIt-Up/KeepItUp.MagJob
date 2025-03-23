using KeepItUp.MagJob.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.ContributorAggregate.Events;

/// <summary>
/// Zdarzenie domenowe informujące o usunięciu kontrybutora.
/// </summary>
public class ContributorDeletedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator kontrybutora.
    /// </summary>
    public Guid ContributorId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie ContributorDeletedEvent.
    /// </summary>
    /// <param name="contributorId">Identyfikator kontrybutora.</param>
    public ContributorDeletedEvent(Guid contributorId)
    {
        ContributorId = contributorId;
    }
}
