using LanPlanner.Application.Interfaces;

namespace LanPlanner.Application.Features.Users.GetCurrentUser;

public record GetCurrentUserQuery(string UserEmail) : IQuery<GetCurrentUserQueryResult>;