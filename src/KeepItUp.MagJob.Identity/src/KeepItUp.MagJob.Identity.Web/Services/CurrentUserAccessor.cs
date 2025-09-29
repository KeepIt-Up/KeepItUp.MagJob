using System.Security.Claims;

namespace KeepItUp.MagJob.Identity.Web.Services;

/// <summary>
/// Provides access to information about the current user.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets the identifier of the current user.
    /// </summary>
    /// <returns>User identifier or null if the user is not logged in.</returns>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Gets the identifier of the current user or throws an exception if the user is not logged in.
    /// </summary>
    /// <returns>User identifier.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not logged in.</exception>
    Guid GetRequiredCurrentUserId();
}

/// <summary>
/// Implementation of access to information about the current user.
/// </summary>
public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentUserAccessor"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Access to the HTTP context.</param>
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
