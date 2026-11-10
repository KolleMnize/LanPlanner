using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameRatingRemoved(GameId GameId, UserId UserId) : IDomainEvent;