using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record LanPartyDetailsChanged(
LanPartyId LanPartyId, string Name, DateOnly Start, DateOnly End) : IDomainEvent;