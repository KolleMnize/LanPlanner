using ErrorOr;

namespace LanPlanner.Domain.Games;

public static class GameErrors
{
    // GameId value object validation errors
    public static readonly Error GameIdCannotBeEmpty = Error.Validation(
        code: "Game.GameIdCannotBeEmpty",
        description: "GameId cannot be empty."
    );

    // GameName value object validation errors
    public static readonly Error NameRequired = Error.Validation(
        code: "Game.NameRequired",
        description: "Game name is required."
    );

    public static readonly Error NameTooLong = Error.Validation(
        code: "Game.NameTooLong",
        description: "Game name is too long."
    );

    public static readonly Error NameAlreadyTaken = Error.Conflict(
        code: "Game.NameAlreadyTaken",
        description: "Game name is already taken."
    ); // used from application layer to indicate name conflicts

    // MaxPlayers value object validation errors
    public static readonly Error MaxPlayersInvalid = Error.Validation(
        code: "Game.MaxPlayersInvalid",
        description: "Max players value is invalid."
    );

    //GameMode value object validation errors
    public static readonly Error GameModeIdCannotBeEmpty = Error.Validation(
    code: "GameMode.GameModeIdCannotBeEmpty",
    description: "GameModeId cannot be empty."
    );

    public static readonly Error GameModeNameRequired = Error.Validation(
        code: "Game.ModeNameRequired",
        description: "Name is required."
    );

    public static readonly Error GameModeNameTooLong = Error.Validation(
        code: "Game.ModeNameTooLong",
        description: "Name is too long."
    );
}
