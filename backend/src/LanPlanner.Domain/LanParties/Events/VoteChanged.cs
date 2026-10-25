using LanPlanner.Domain.Games;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record VoteChanged(LanPartyId LanPartyId, UserId UserId, GameModeId GameModeId, VoteDirection NewDirection) : IDomainEvent;