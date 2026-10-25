using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record ParticipantLeft(LanPartyId LanPartyId, UserId UserId) : IDomainEvent;