using System.Security.Claims;

namespace LanPlanner.Api.Authentication;

public sealed class HttpContextCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string? Email => accessor.HttpContext?.User.FindFirstValue("email");
}