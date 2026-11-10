using ErrorOr;

namespace LanPlanner.Domain.Games;

public sealed record Stars
{
    public const int MinValue = 1;
    public const int MaxValue = 5;
    public int Value { get; private init; }

    private Stars(int value) => Value = value; // auch für EF Core nutzbar

    public static ErrorOr<Stars> Create(int value)
    {
        if (value < MinValue || value > MaxValue)
            return GameErrors.StarsOutOfRange;

        return new Stars(value);
    }
}