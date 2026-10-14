using LanPlanner.Api.Authentication;
using LanPlanner.Api.Extensions;
using LanPlanner.Application.Features.Users.GetCurrentUser;
using LanPlanner.Application.Features.Users.RegisterUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserManagementController(ICurrentUser currentUser, GetCurrentUserHandler getCurrentUserHandler, RegisterUserHandler registerUserHandler) : ControllerBase
{
    [HttpGet("me", Name = "GetCurrentUser")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await getCurrentUserHandler.Handle(new GetCurrentUserQuery(currentUser.Email ?? string.Empty));
        return result.ToProblemDetails(this);
    }

    [HttpPost("me", Name = "RegisterUser")]
    public async Task<IActionResult> Register([FromBody] string Name, CancellationToken ct)
    {
        var result = await registerUserHandler.Handle(
            new RegisterUserCommand(currentUser.Email ?? string.Empty, Name), ct);
        return result.ToProblemDetails(this);
    }
}
