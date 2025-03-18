using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Reprezentuje rolę w organizacji.
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Opis roli.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Kolor roli (w formacie HEX).
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Identyfikator organizacji, do której należy rola.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Lista uprawnień przypisanych do roli.
    /// </summary>
    private readonly List<Permission> _permissions = new();

    /// <summary>
    /// Lista uprawnień przypisanych do roli (tylko do odczytu).
    /// </summary>
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    /// <summary>
    /// Lista członków posiadających tę rolę.
    /// </summary>
    private readonly List<Member> _members = new();

    /// <summary>
    /// Lista członków posiadających tę rolę (tylko do odczytu).
    /// </summary>
    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();

    // Prywatny konstruktor dla EF Core
    private Role() { }

    /// <summary>
    /// Tworzy nową rolę.
    /// </summary>
    /// <param name="name">Nazwa roli.</param>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="description">Opis roli.</param>
    /// <param name="color">Kolor roli (w formacie HEX).</param>
    /// <returns>Nowa rola.</returns>
    public static Role Create(string name, Guid organizationId, string? description = null, string? color = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.Default(organizationId, nameof(organizationId));

        return new Role
        {
            Name = name,
            OrganizationId = organizationId,
            Description = description,
            Color = color
        };
    }

    /// <summary>
    /// Aktualizuje dane roli.
    /// </summary>
    /// <param name="name">Nazwa roli.</param>
    /// <param name="description">Opis roli.</param>
    /// <param name="color">Kolor roli (w formacie HEX).</param>
    public void Update(string name, string? description = null, string? color = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
        Description = description;
        Color = color;

        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }

    /// <summary>
    /// Dodaje uprawnienie do roli.
    /// </summary>
    /// <param name="permission">Uprawnienie do dodania.</param>
    public void AddPermission(Permission permission)
    {
        Guard.Against.Null(permission, nameof(permission));

        // Sprawdź, czy uprawnienie już istnieje
        if (_permissions.Any(p => p.Name == permission.Name))
        {
            return;
        }

        _permissions.Add(permission);

        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }

    /// <summary>
    /// Usuwa uprawnienie z roli.
    /// </summary>
    /// <param name="permissionName">Nazwa uprawnienia do usunięcia.</param>
    public void RemovePermission(string permissionName)
    {
        Guard.Against.NullOrEmpty(permissionName, nameof(permissionName));

        // Znajdź uprawnienie
        var permission = _permissions.FirstOrDefault(p => p.Name == permissionName);
        if (permission == null)
        {
            return;
        }

        _permissions.Remove(permission);

        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }

    /// <summary>
    /// Sprawdza, czy rola posiada określone uprawnienie.
    /// </summary>
    /// <param name="permissionName">Nazwa uprawnienia.</param>
    /// <returns>True, jeśli rola posiada uprawnienie; w przeciwnym razie false.</returns>
    public bool HasPermission(string permissionName)
    {
        Guard.Against.NullOrEmpty(permissionName, nameof(permissionName));

        return _permissions.Any(p => p.Name == permissionName);
    }

    /// <summary>
    /// Usuwa wszystkie uprawnienia z roli.
    /// </summary>
    public void ClearPermissions()
    {
        _permissions.Clear();

        // Wywołanie metody Update z klasy bazowej
        base.Update();
    }
}
