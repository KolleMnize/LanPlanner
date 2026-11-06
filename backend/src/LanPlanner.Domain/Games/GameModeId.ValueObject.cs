using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games;

public sealed class GameModeId : EntityId
{
    private GameModeId(Guid value) : base(value) { }

    internal static GameModeId NewForDefault() => Create(Guid.NewGuid()).Value;
    internal static GameModeId NewForAddMode() => Create(Guid.NewGuid()).Value;
    public static ErrorOr<GameModeId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GameErrors.GameModeIdCannotBeEmpty;

        return new GameModeId(value);
    }
}
