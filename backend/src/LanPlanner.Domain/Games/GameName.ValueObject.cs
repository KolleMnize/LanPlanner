using ErrorOr;

namespace LanPlanner.Domain.Games;

public sealed record GameName
{
    public const int MaxLength = 100;

    public string Value { get; private init; }
    public string Normalized => Value.ToUpperInvariant();

    private GameName(string value) => Value = value; // auch für EF Core nutzbar

    public static ErrorOr<GameName> Create(string value)
    {
        var cleanedValue = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleanedValue))
            return GameErrors.NameRequired;

        if (cleanedValue.Length > MaxLength)
            return GameErrors.NameTooLong;

        return new GameName(cleanedValue);

    }
}
