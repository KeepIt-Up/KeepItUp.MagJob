using Ardalis.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.SharedKernel;

/// <summary>
/// Klasa bazowa dla wszystkich zdarzeń domenowych w systemie.
/// </summary>
public abstract class DomainEventBase : Ardalis.SharedKernel.DomainEventBase
{
    // Klasa DomainEventBase z Ardalis.SharedKernel już implementuje INotification
    // i zawiera DateOccurred
} 
