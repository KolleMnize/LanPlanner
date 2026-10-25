using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.LanParties;

public class Proposal
{
    public required GameModeId GameModeId { get; init; }
    private Proposal() { } // Required by EF Core or other ORMs

    internal static Proposal Create(GameModeId gameModeId)
        => new Proposal { GameModeId = gameModeId };
}
