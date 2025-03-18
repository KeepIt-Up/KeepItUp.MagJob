using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Reprezentuje członka organizacji.
/// </summary>
public class Member : BaseEntity
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Lista identyfikatorów ról przypisanych do członka.
    /// </summary>
    private readonly List<Guid> _roleIds = new();

    /// <summary>
    /// Lista identyfikatorów ról przypisanych do członka (tylko do odczytu).
    /// </summary>
    public IReadOnlyCollection<Guid> RoleIds => _roleIds.AsReadOnly();

    /// <summary>
    /// Lista ról przypisanych do członka (właściwość nawigacyjna dla EF Core).
    /// </summary>
    public virtual ICollection<Role> Roles { get; private set; } = new List<Role>();

    /// <summary>
    /// Data dołączenia do organizacji.
    /// </summary>
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    // Prywatny konstruktor dla EF Core
    private Member() { }

    /// <summary>
    /// Tworzy nowego członka organizacji.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="roleId">Identyfikator początkowej roli.</param>
    /// <returns>Nowy członek organizacji.</returns>
    public static Member Create(Guid userId, Guid organizationId, Guid roleId)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(organizationId, nameof(organizationId));
        Guard.Against.Default(roleId, nameof(roleId));

        var member = new Member
        {
            UserId = userId,
            OrganizationId = organizationId
        };

        member._roleIds.Add(roleId);

        return member;
    }

    /// <summary>
    /// Przypisuje nową rolę członkowi organizacji.
    /// </summary>
    /// <param name="roleId">Identyfikator roli do przypisania.</param>
    public void AssignRole(Guid roleId)
    {
        Guard.Against.Default(roleId, nameof(roleId));

        if (!_roleIds.Contains(roleId))
        {
            _roleIds.Add(roleId);

            // Wywołanie metody Update z klasy bazowej
            base.Update();
        }
    }

    /// <summary>
    /// Usuwa rolę przypisaną do członka organizacji.
    /// </summary>
    /// <param name="roleId">Identyfikator roli do usunięcia.</param>
    /// <returns>True, jeśli rola została usunięta; w przeciwnym razie false.</returns>
    public bool RemoveRole(Guid roleId)
    {
        Guard.Against.Default(roleId, nameof(roleId));

        // Nie pozwól na usunięcie ostatniej roli
        if (_roleIds.Count <= 1)
        {
            return false;
        }

        bool removed = _roleIds.Remove(roleId);

        if (removed)
        {
            // Wywołanie metody Update z klasy bazowej
            base.Update();
        }

        return removed;
    }

    /// <summary>
    /// Sprawdza, czy członek posiada określoną rolę.
    /// </summary>
    /// <param name="roleId">Identyfikator roli.</param>
    /// <returns>True, jeśli członek posiada rolę; w przeciwnym razie false.</returns>
    public bool HasRole(Guid roleId)
    {
        return _roleIds.Contains(roleId);
    }
}
