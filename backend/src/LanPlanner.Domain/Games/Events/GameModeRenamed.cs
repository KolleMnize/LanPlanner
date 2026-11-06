using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameModeRenamed(GameId GameId, GameModeId GameModeId, string GameModeName) : IDomainEvent;
