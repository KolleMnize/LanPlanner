using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record LanPartyCreated(
    LanPartyId LanPartyId, string Name, DateOnly Start, DateOnly End) : IDomainEvent;