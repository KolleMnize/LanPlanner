using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games;

public class GameModeId : EntityId
{
    public GameModeId(Guid value) : base(value) { }

    public static ErrorOr<GameModeId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GameModeErrors.GameModeIdCannotBeEmpty;

        return new GameModeId(value);
    }
}
