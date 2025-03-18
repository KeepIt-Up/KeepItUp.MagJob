using System.Security.Claims;

namespace KeepItUp.MagJob.Identity.Web.Services;

/// <summary>
/// Dostarcza dostęp do informacji o bieżącym użytkowniku.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Pobiera identyfikator bieżącego użytkownika.
    /// </summary>
    /// <returns>Identyfikator użytkownika lub null, jeśli użytkownik nie jest zalogowany.</returns>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Pobiera identyfikator bieżącego użytkownika lub zgłasza wyjątek, jeśli użytkownik nie jest zalogowany.
    /// </summary>
    /// <returns>Identyfikator użytkownika.</returns>
    /// <exception cref="UnauthorizedAccessException">Zgłaszany, gdy użytkownik nie jest zalogowany.</exception>
    Guid GetRequiredCurrentUserId();
}

/// <summary>
/// Implementacja dostępu do informacji o bieżącym użytkowniku.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CurrentUserAccessor"/>.
    /// </summary>
    /// <param name="httpContextAccessor">Dostęp do kontekstu HTTP.</param>
    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
        {
            return null;
        }

        return userGuid;
    }

    /// <inheritdoc />
    public Guid GetRequiredCurrentUserId()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Użytkownik nie jest zalogowany lub nie można zidentyfikować użytkownika.");
        }

        return userId.Value;
    }
}
