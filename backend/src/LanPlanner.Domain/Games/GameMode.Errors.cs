using ErrorOr;

namespace LanPlanner.Domain.Games;

public static class GameModeErrors
{
    public static readonly Error GameModeIdCannotBeEmpty = Error.Validation(
        code: "GameMode.GameModeIdCannotBeEmpty",
        description: "GameModeId cannot be empty."
    );
}
