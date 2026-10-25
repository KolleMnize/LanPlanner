using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class Vote
{
    public required UserId UserId { get; init; }
    public VoteDirection Direction { get; private set; }
    private Vote() { } // Required by EF Core or other ORMs

    internal static Vote Create(UserId userId, VoteDirection direction)
        => new Vote { UserId = userId, Direction = direction };

    internal void Flip() => Direction = Direction == VoteDirection.Up ? VoteDirection.Down : VoteDirection.Up;
}