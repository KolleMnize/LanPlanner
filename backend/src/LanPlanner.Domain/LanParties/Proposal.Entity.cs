using LanPlanner.Domain.Games;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class Proposal
{
    private List<Vote> _votes = [];
    public required GameModeId GameModeId { get; init; }
    private Proposal() { } // Required by EF Core or other ORMs

    internal static Proposal Create(GameModeId gameModeId)
        => new Proposal { GameModeId = gameModeId };

    internal void AddVote(Vote vote)
        => _votes.Add(vote);
    internal Vote? GetVote(UserId userId)
        => _votes.FirstOrDefault(v => v.UserId == userId);
}
