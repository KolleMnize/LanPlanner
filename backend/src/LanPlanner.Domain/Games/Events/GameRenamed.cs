using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record class GameRenamed(GameId GameId, string Name) : IDomainEvent;
