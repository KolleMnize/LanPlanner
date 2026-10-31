using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public partial class GameNameValueObjectTests
{

    [TestMethod]
    public void Create_NameWithLeadingAndTrailingSpaces_ReturnsGameName()
    {
        var name = "      Valid Name   ";
        var trimmed = name.Trim();
        var normalized = trimmed.ToUpper();

        var result = GameName.Create(name);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(trimmed, result.Value.Value);
        Assert.AreEqual(normalized, result.Value.Normalized);
    }

    [TestMethod]
    public void Create_NameWithMaxLength_ReturnsGameName()
    {
        var name = new string('a', GameName.MaxLength);
        var trimmed = name.Trim();
        var normalized = trimmed.ToUpper();

        var result = GameName.Create(name);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(trimmed, result.Value.Value);
        Assert.AreEqual(normalized, result.Value.Normalized);
    }

    [TestMethod]
    public void Create_EmptyName_ReturnsError()
    {
        var name = "   ";

        var result = GameName.Create(name);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameIsNull_ReturnsError()
    {
        var result = GameName.Create(null);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameTooLong_ReturnsError()
    {
        var name = new string('a', GameName.MaxLength + 1);

        var result = GameName.Create(name);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.NameTooLong, result.FirstError);
    }
}