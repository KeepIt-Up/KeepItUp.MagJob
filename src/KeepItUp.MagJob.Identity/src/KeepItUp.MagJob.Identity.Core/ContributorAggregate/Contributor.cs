using KeepItUp.MagJob.Identity.Core.ContributorAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.ContributorAggregate;

/// <summary>
/// Reprezentuje kontrybutora w systemie.
/// </summary>
public class Contributor : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Nazwa kontrybutora.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Status kontrybutora.
    /// </summary>
    public ContributorStatus Status { get; private set; } = ContributorStatus.NotSet;

    /// <summary>
    /// Numer telefonu kontrybutora.
    /// </summary>
    public PhoneNumber? PhoneNumber { get; private set; }

    /// <summary>
    /// Prywatny konstruktor dla EF Core oraz tworzenia przez fabrykę.
    /// </summary>
    private Contributor() { }

    /// <summary>
    /// Tworzy nowego kontrybutora.
    /// </summary>
    /// <param name="name">Nazwa kontrybutora.</param>
    /// <returns>Nowy kontrybutor.</returns>
    public static Contributor Create(string name)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        var contributor = new Contributor
        {
            Name = name
        };

        contributor.RegisterDomainEventAndUpdate(new ContributorCreatedEvent(contributor.Id, name));

        return contributor;
    }

    /// <summary>
    /// Ustawia numer telefonu kontrybutora.
    /// </summary>
    /// <param name="phoneNumber">Numer telefonu.</param>
    public void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = new PhoneNumber(string.Empty, phoneNumber, string.Empty);
        RegisterDomainEventAndUpdate(new ContributorUpdatedEvent(Id, Name));
    }

    /// <summary>
    /// Aktualizuje nazwę kontrybutora.
    /// </summary>
    /// <param name="newName">Nowa nazwa kontrybutora.</param>
    public void UpdateName(string newName)
    {
        Guard.Against.NullOrEmpty(newName, nameof(newName));
        Name = newName;
        RegisterDomainEventAndUpdate(new ContributorUpdatedEvent(Id, Name));
    }

    /// <summary>
    /// Aktualizuje status kontrybutora.
    /// </summary>
    /// <param name="status">Nowy status kontrybutora.</param>
    public void UpdateStatus(ContributorStatus status)
    {
        Status = status;
        RegisterDomainEventAndUpdate(new ContributorStatusUpdatedEvent(Id, Name, Status));
    }

    /// <summary>
    /// Oznacza kontrybutora jako usunięty i rejestruje odpowiednie zdarzenie domenowe.
    /// </summary>
    public void Delete()
    {
        RegisterDomainEventAndUpdate(new ContributorDeletedEvent(Id));
    }
}

/// <summary>
/// Reprezentuje numer telefonu jako obiekt wartości.
/// </summary>
public class PhoneNumber : ValueObject
{
    /// <summary>
    /// Kod kraju.
    /// </summary>
    public string CountryCode { get; private set; }

    /// <summary>
    /// Numer telefonu.
    /// </summary>
    public string Number { get; private set; }

    /// <summary>
    /// Rozszerzenie numeru telefonu.
    /// </summary>
    public string? Extension { get; private set; }

    /// <summary>
    /// Tworzy nowy numer telefonu.
    /// </summary>
    /// <param name="countryCode">Kod kraju.</param>
    /// <param name="number">Numer telefonu.</param>
    /// <param name="extension">Rozszerzenie numeru telefonu.</param>
    public PhoneNumber(string countryCode, string number, string? extension)
    {
        CountryCode = countryCode;
        Number = number;
        Extension = extension;
    }

    /// <summary>
    /// Zwraca komponenty do porównania równości obiektów.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
        yield return Extension ?? String.Empty;
    }
}
