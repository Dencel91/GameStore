using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameStore.Common.Helpers;

public static class AuthHelper
{
    public static Guid GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException("No HttpContext");
        }

        //var userId = httpContextAccessor.HttpContext?.User?.Identity?.Name;

        string userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("No Identifier found in claims");

        return Guid.Parse(userId);
    }

    public static Guid GetCurrentUserIdOrDefault(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return Guid.Empty;
        }

        var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return string.IsNullOrEmpty(userId) ? Guid.Empty : Guid.Parse(userId);
    }
}
