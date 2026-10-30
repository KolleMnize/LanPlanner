using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Users.Events;

public sealed record UserCreated(UserId UserId, string Name) : IDomainEvent;