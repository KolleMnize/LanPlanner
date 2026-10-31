using ErrorOr;

namespace LanPlanner.Domain.Games;

public sealed record GameModeName
{
    public const int MaxLength = 50;

    public string Value { get; private init; }
    public string Normalized => Value.ToUpperInvariant();

    private GameModeName(string value) => Value = value; // auch für EF Core nutzbar

    public static ErrorOr<GameModeName> Create(string value)
    {
        var cleanedValue = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleanedValue))
            return GameErrors.GameModeNameRequired;

        if (cleanedValue.Length > MaxLength)
            return GameErrors.GameModeNameTooLong;

        return new GameModeName(cleanedValue);

    }
}
