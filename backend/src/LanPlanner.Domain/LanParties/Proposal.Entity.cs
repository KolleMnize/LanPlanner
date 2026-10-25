using LanPlanner.Domain.Games;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class Proposal
{
    private List<Vote> _votes = [];
    private List<UserId> _vetos = [];

    public required GameModeId GameModeId { get; init; }
    private Proposal() { } // Required by EF Core or other ORMs

    internal static Proposal Create(GameModeId gameModeId)
        => new Proposal { GameModeId = gameModeId };

    internal void AddVote(Vote vote)
        => _votes.Add(vote);

    internal void RemoveVote(Vote vote)
        => _votes.Remove(vote);
    internal Vote? FindVote(UserId userId)
        => _votes.FirstOrDefault(v => v.UserId == userId);
    internal void AddVeto(UserId userId)
        => _vetos.Add(userId);
    internal bool HasVetoFrom(UserId userId)
        => _vetos.Contains(userId);
    internal void RemoveVeto(UserId userId)
        => _vetos.Remove(userId);
}
