using LanPlanner.Domain.Games;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record ProposalRemoved(LanPartyId LanPartyId, UserId RemovedBy, GameModeId GameModeId) : IDomainEvent;