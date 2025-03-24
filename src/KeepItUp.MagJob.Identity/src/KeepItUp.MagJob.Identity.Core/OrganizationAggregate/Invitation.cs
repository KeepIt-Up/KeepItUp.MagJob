using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Status zaproszenia.
/// </summary>
public enum InvitationStatus
{
    /// <summary>
    /// Zaproszenie oczekuje na akceptację.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Zaproszenie zostało zaakceptowane.
    /// </summary>
    Accepted = 1,

    /// <summary>
    /// Zaproszenie zostało odrzucone.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Zaproszenie wygasło.
    /// </summary>
    Expired = 3
}

/// <summary>
/// Reprezentuje zaproszenie do organizacji.
/// </summary>
public class Invitation : BaseEntity
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Adres e-mail zapraszanego użytkownika.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Token zaproszenia.
    /// </summary>
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// Identyfikator roli, która zostanie przypisana po akceptacji zaproszenia.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Status zaproszenia.
    /// </summary>
    public InvitationStatus Status { get; private set; } = InvitationStatus.Pending;

    /// <summary>
    /// Data wygaśnięcia zaproszenia.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Czy zaproszenie wygasło.
    /// </summary>
    public bool IsExpired => Status == InvitationStatus.Expired || DateTime.UtcNow > ExpiresAt;

    // Prywatny konstruktor dla EF Core
    private Invitation() { }

    /// <summary>
    /// Tworzy nowe zaproszenie do organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="email">Adres e-mail osoby zapraszanej.</param>
    /// <param name="roleId">Identyfikator roli, która zostanie przypisana po akceptacji zaproszenia.</param>
    /// <param name="expiresAt">Data wygaśnięcia zaproszenia.</param>
    /// <returns>Nowe zaproszenie.</returns>
    public static Invitation Create(Guid organizationId, string email, Guid roleId, DateTime? expiresAt = null)
    {
        Guard.Against.Default(organizationId, nameof(organizationId));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.Default(roleId, nameof(roleId));

        return new Invitation
        {
            OrganizationId = organizationId,
            Email = email,
            Token = Guid.NewGuid().ToString(),
            RoleId = roleId,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
        };
    }

    /// <summary>
    /// Akceptuje zaproszenie.
    /// </summary>
    public void Accept()
    {
        if (IsExpired)
        {
            throw new InvalidOperationException("Nie można zaakceptować wygasłego zaproszenia.");
        }

        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Zaproszenie zostało już zaakceptowane lub odrzucone.");
        }

        Status = InvitationStatus.Accepted;
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }

    /// <summary>
    /// Odrzuca zaproszenie.
    /// </summary>
    public void Reject()
    {
        if (IsExpired)
        {
            throw new InvalidOperationException("Nie można odrzucić wygasłego zaproszenia.");
        }

        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Zaproszenie zostało już zaakceptowane lub odrzucone.");
        }

        Status = InvitationStatus.Rejected;
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }

    /// <summary>
    /// Oznacza zaproszenie jako wygasłe.
    /// </summary>
    public void MarkAsExpired()
    {
        if (Status != InvitationStatus.Pending)
        {
            return;
        }

        Status = InvitationStatus.Expired;
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }
} 
