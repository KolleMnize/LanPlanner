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

    public static readonly Error GameModeNameAlreadyTaken = Error.Conflict(
        code: "Game.ModeNameAlreadyTaken",
        description: "Name is already taken."
    );

    public static readonly Error GameModeNameUnchanged = Error.Conflict(
        code: "Game.ModeNameUnchanged",
        description: "The new Name is the same as the current one."
    );

    public static readonly Error GameModeNotFound = Error.NotFound(
        code: "Game.ModeNotFound",
        description: "Game mode not found."
    );

    public static readonly Error DefaultModeCannotBeRenamed = Error.Conflict(
        code: "Game.DefaultModeCannotBeRenamed",
        description: "The default game mode cannot be renamed."
    );

    public static readonly Error GameModeMaxPlayersUnchanged = Error.Conflict(
        code: "Game.ModeMaxPlayersUnchanged",
        description: "The new MaxPlayers value is the same as the current one."
    );

    //Game Errors
    public static readonly Error GameNameUnchanged = Error.Conflict(
        code: "Game.NameUnchanged",
        description: "The new Name is the same as the current one."
    );


    // Stars value object validation errors
    public static readonly Error StarsOutOfRange = Error.Validation(
        code: "Game.StarsOutOfRange",
        description: "Stars value is out of range."
    );

    // Rating errors
    public static readonly Error ActorAlreadyRated = Error.Conflict(
        code: "Game.ActorAlreadyRated",
        description: "The actor has already rated this game."
    );

    public static readonly Error ActorRatingNotFound = Error.NotFound(
        code: "Game.ActorRatingNotFound",
        description: "The actor's rating was not found."
    );

    public static readonly Error RatingUnchanged = Error.Conflict(
        code: "Game.RatingUnchanged",
        description: "The new rating is the same as the current one."
    );
}
