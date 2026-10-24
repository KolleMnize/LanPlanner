using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record ParticipantJoined(
    LanPartyId LanPartyId, UserId UserId) : IDomainEvent;