using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameModeAdded(GameId GameId, GameModeId GameModeId, string GameModeName, int MaxPlayers) : IDomainEvent;