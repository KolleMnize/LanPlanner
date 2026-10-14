
using ErrorOr;
using LanPlanner.Application.Interfaces;
using LanPlanner.Domain.Users;

namespace LanPlanner.Application.Features.Users.GetCurrentUser;

public class GetCurrentUserHandler(IUserRepository userRepository) : IQueryHandler<GetCurrentUserQuery, GetCurrentUserQueryResult>
{
    public async Task<ErrorOr<GetCurrentUserQueryResult>> Handle(GetCurrentUserQuery query, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(query.UserEmail))
        {
            return Error.Unauthorized(
                code: "User.NotAuthenticated",
                description: "Es konnte kein angemeldeter Nutzer ermittelt werden.");
        }

        var existing = await userRepository.GetByEmail(query.UserEmail, ct);

        if (existing.IsError)
            return existing.Errors;

        if (existing.Value is null)
            return Error.NotFound(
                code: "User.NotFound",
                description: "Der Nutzer ist noch nicht registriert.");

        var user = existing.Value;
        return new GetCurrentUserQueryResult(UserDtoMapper.UserToUserDto(user));
    }
}
