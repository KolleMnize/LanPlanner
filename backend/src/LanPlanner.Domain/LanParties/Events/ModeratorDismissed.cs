using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public record ModeratorDismissed(
    LanPartyId LanPartyId, UserId UserId) : IDomainEvent;