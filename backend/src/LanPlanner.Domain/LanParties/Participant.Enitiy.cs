using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class Participant
{
    public required UserId UserId { get; init; }
    public bool IsModerator { get; private set; } = false;

    private Participant() { } // Required by EF Core or other ORMs

    internal static Participant Create(UserId userId)
        => new Participant { UserId = userId, IsModerator = false };
    internal void Promote() => IsModerator = true;
    internal void Demote() => IsModerator = false;
}