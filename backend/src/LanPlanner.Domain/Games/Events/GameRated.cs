using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameRated(GameId GameId, UserId UserId, int Stars) : IDomainEvent;