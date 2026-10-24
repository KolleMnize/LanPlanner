using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.LanParties.Events;

public record VotingClosed(
LanPartyId LanPartyId) : IDomainEvent;