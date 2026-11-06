using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameModeMaxPlayersChanged(GameId GameId, GameModeId GameModeId, int MaxPlayers) : IDomainEvent;