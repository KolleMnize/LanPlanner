using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public class GameModeNameValueObjectTests
{
    [TestMethod]
    public void Create_NameWithLeadingAndTrailingSpaces_ReturnsGameModeName()
    {
        var name = "    Valid Name   ";
        var trimmed = name.Trim();
        var normalized = trimmed.ToUpper();

        var result = GameModeName.Create(name);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(trimmed, result.Value.Value);
        Assert.AreEqual(normalized, result.Value.Normalized);
    }

    [TestMethod]
    public void Create_NameWithMaxLength_ReturnsGameModeName()
    {
        var name = new string('a', GameModeName.MaxLength);
        var trimmed = name.Trim();
        var normalized = trimmed.ToUpper();

        var result = GameModeName.Create(name);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(trimmed, result.Value.Value);
        Assert.AreEqual(normalized, result.Value.Normalized);
    }

    [TestMethod]
    public void Create_EmptyName_ReturnsError()
    {
        var name = "    ";

        var result = GameModeName.Create(name);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.GameModeNameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameIsNull_ReturnsError()
    {
        var result = GameModeName.Create(null);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.GameModeNameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameTooLong_ReturnsError()
    {
        var name = new string('a', GameModeName.MaxLength + 1);

        var result = GameModeName.Create(name);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.GameModeNameTooLong, result.FirstError);
    }
}
