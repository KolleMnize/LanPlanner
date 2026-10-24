using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record LanPartyEnded(
    LanPartyId LanPartyId) : IDomainEvent;