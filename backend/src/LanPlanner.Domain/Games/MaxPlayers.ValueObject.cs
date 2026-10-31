using ErrorOr;

namespace LanPlanner.Domain.Games;

public sealed record class MaxPlayers
{
    public int Value { get; private init; }

    private MaxPlayers(int value) => Value = value; // auch für EF Core nutzbar

    public static ErrorOr<MaxPlayers> Create(int value)
    {
        if (value <= 0)
            return GameErrors.MaxPlayersInvalid;

        return new MaxPlayers(value);
    }
}
