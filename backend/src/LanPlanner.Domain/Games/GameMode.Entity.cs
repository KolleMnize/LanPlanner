using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games;

public sealed class GameMode
{
    public required GameModeId Id { get; init; }
    public GameModeName Name { get; private set; }
    public MaxPlayers MaxPlayers { get; private set; }
    public bool IsDefault { get; private set; }

    private GameMode() // for ORM
    {
        Name = null!;
        MaxPlayers = null!;
    }

    internal static GameMode CreateDefault(MaxPlayers maxPlayers)
    {
        var gameModeId = GameModeId.NewForDefault();

        var gameMode = new GameMode
        {
            Id = gameModeId,
            Name = GameModeName.Default,
            MaxPlayers = maxPlayers,
            IsDefault = true
        };

        return gameMode;
    }

    internal static GameMode Create(GameModeId id, GameModeName name, MaxPlayers maxPlayers)
    {
        var gameMode = new GameMode
        {
            Id = id,
            Name = name,
            MaxPlayers = maxPlayers,
            IsDefault = false
        };

        return gameMode;
    }

    internal void Rename(GameModeName name)
    {
        Name = name;
    }

    internal void ChangeMaxPlayers(MaxPlayers maxPlayers)
    {
        MaxPlayers = maxPlayers;
    }

}