using LanPlanner.Api.Authentication;
using LanPlanner.Api.Extensions;
using LanPlanner.Application.Features.Users.GetCurrentUser;
using Microsoft.AspNetCore.Mvc;

namespace LanPlanner.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserManagementController(ICurrentUser currentUser, GetCurrentUserHandler getCurrentUserHandler) : ControllerBase
{
    [HttpGet("me", Name = "GetCurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await getCurrentUserHandler.Handle(new GetCurrentUserQuery(currentUser.Email ?? string.Empty));
        return result.ToProblemDetails(this);
    }
}
