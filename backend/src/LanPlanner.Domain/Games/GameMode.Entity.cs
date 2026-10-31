using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games;

public sealed class GameMode
{
    public required GameModeId Id { get; init; }
    public required GameModeName Name { get; init; }
    public required MaxPlayers MaxPlayers { get; init; }
    public bool IsDefault { get; private set; }

    private GameMode() { } // for ORM

    internal static GameMode CreateDefault(GameModeId id, MaxPlayers maxPlayers)
    {
        var gameMode = new GameMode
        {
            Id = id,
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

}