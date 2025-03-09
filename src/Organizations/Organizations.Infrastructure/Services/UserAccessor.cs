using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Organizations.Application.Common.Interfaces;

namespace Organizations.Infrastructure.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        return Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
    }

    public string GetUserName()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    public string GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public string GetFirstName()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
    }

    public string GetLastName()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
    }
}