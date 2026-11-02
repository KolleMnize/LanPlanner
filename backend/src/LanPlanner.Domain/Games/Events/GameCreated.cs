using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games.Events;

public sealed record class GameCreated(GameId Id, GameName Name) : IDomainEvent;