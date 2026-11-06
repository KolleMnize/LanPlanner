using ErrorOr;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games;

public class Game : Aggregate<GameId>
{
    private List<GameMode> gameModes = [];
    public GameName Name { get; private set; } = null!;
    public IReadOnlyList<GameMode> GameModes => gameModes.ToList().AsReadOnly();

    private Game() { } // Required by EF Core or other ORMs

    public static ErrorOr<Game> Create(GameId id, GameName name, MaxPlayers maxPlayersDefaultGameMode)
    {
        var gameMode = GameMode.CreateDefault(maxPlayersDefaultGameMode);

        var newGame = new Game
        {
            Id = id,
            Name = name,
        };

        newGame.Raise(new GameCreated(newGame.Id, newGame.Name));
        newGame.AddModeInternal(gameMode);
        return newGame;
    }

    public ErrorOr<Success> Rename(string newName)
    {
        var newGameName = GameName.Create(newName);

        if (newGameName.IsError)
            return newGameName.Errors;

        if (Name == newGameName.Value)
            return GameErrors.GameNameUnchanged;

        RenameInternal(newGameName.Value);
        return Result.Success;
    }

    public ErrorOr<Success> AddMode(GameModeName name, MaxPlayers maxPlayers)
    {
        if (IsModeNameTaken(name))
            return GameErrors.GameModeNameAlreadyTaken;

        var gameModeId = GameModeId.NewForAddMode();

        var gameMode = GameMode.Create(gameModeId, name, maxPlayers);

        AddModeInternal(gameMode);
        return Result.Success;
    }

    public ErrorOr<Success> RenameMode(GameModeId gameModeId, GameModeName newName)
    {
        var gameMode = GetModeById(gameModeId);
        if (gameMode is null)
            return GameErrors.GameModeNotFound;

        if (gameMode.IsDefault)
            return GameErrors.DefaultModeCannotBeRenamed;

        if (IsModeNameTaken(newName, gameModeId))
            return GameErrors.GameModeNameAlreadyTaken;

        if (gameMode.Name == newName)
            return GameErrors.GameModeNameUnchanged;

        RenameModeInternal(gameMode, newName);
        return Result.Success;
    }

    public ErrorOr<Success> ChangeMaxPlayers(GameModeId gameModeId, MaxPlayers newMaxPlayers)
    {
        var gameMode = GetModeById(gameModeId);
        if (gameMode is null)
            return GameErrors.GameModeNotFound;

        if (gameMode.MaxPlayers == newMaxPlayers)
            return GameErrors.GameModeMaxPlayersUnchanged;

        ChangeMaxPlayersInternal(gameMode, newMaxPlayers);
        return Result.Success;
    }

    private void AddModeInternal(GameMode gameMode)
    {
        gameModes.Add(gameMode);
        Raise(new GameModeAdded(Id, gameMode.Id, gameMode.Name.Value, gameMode.MaxPlayers.Value));
    }

    private void RenameInternal(GameName newName)
    {
        Name = newName;
        Raise(new GameRenamed(Id, Name.Value));
    }

    private GameMode? GetModeById(GameModeId id)
    {
        return gameModes.FirstOrDefault(m => m.Id == id);
    }
    private bool IsModeNameTaken(GameModeName name, GameModeId? excludeId = null)
    {
        return gameModes.Any(m => m.Name == name && (excludeId == null || m.Id != excludeId));
    }

    private void RenameModeInternal(GameMode gameMode, GameModeName newName)
    {
        gameMode.Rename(newName);
        Raise(new GameModeRenamed(Id, gameMode.Id, newName.Value));
    }

    private void ChangeMaxPlayersInternal(GameMode gameMode, MaxPlayers newMaxPlayers)
    {
        gameMode.ChangeMaxPlayers(newMaxPlayers);
        Raise(new GameModeMaxPlayersChanged(Id, gameMode.Id, newMaxPlayers.Value));
    }
}
