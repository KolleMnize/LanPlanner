using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record VotingOpened(
    LanPartyId LanPartyId) : IDomainEvent;