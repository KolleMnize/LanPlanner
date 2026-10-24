using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record ModeratorAppointed(
    LanPartyId LanPartyId, UserId UserId) : IDomainEvent;