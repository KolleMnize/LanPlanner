using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Games;

public sealed record Rating(UserId UserId, Stars Stars);