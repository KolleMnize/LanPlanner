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

        RenameInteral(newGameName.Value);
        return Result.Success;
    }

    private void AddModeInternal(GameMode gameMode)
    {
        gameModes.Add(gameMode);
        Raise(new GameModeAdded(Id, gameMode.Id, gameMode.Name.Value, gameMode.MaxPlayers.Value));
    }

    private void RenameInteral(GameName newName)
    {
        Name = newName;
        Raise(new GameRenamed(Id, Name.Value));
    }
}
