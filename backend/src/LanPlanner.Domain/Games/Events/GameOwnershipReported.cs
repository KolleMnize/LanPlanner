using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameOwnershipReported(GameId GameId, UserId UserId) : IDomainEvent;