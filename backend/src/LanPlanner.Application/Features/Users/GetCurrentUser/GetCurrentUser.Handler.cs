
using ErrorOr;
using LanPlanner.Application.Interfaces;

namespace LanPlanner.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler : IQueryHandler<GetCurrentUserQuery, GetCurrentUserQueryResult>
{
    public async Task<ErrorOr<GetCurrentUserQueryResult>> Handle(GetCurrentUserQuery query, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(query.UserEmail))
        {
            return Error.Unauthorized(
                code: "User.NotAuthenticated",
                description: "Es konnte kein angemeldeter Nutzer ermittelt werden.");
        }

        return new GetCurrentUserQueryResult(new UserDto(query.UserEmail));
    }
}
