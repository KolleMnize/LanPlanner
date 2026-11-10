using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameModeRemoved(GameId GameId, GameModeId GameModeId) : IDomainEvent;