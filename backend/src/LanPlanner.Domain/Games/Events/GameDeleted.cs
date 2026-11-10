using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameDeleted(GameId GameId) : IDomainEvent;
