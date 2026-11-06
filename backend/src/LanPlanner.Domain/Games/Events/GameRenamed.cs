using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record GameRenamed(GameId GameId, string Name) : IDomainEvent;
