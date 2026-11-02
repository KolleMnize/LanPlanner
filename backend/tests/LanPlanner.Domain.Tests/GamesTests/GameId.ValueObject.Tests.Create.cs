using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public partial class GameIdValueObjectTests
{
    [TestMethod]
    public void Create_ValidGuid_ReturnsGameId()
    {
        var guid = Guid.NewGuid();

        var result = GameId.Create(guid);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(guid, result.Value.Value);
    }

    [TestMethod]
    public void Create_InvalidGuid_ReturnsError()
    {
        var guid = Guid.Empty;

        var result = GameId.Create(guid);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.GameIdCannotBeEmpty, result.FirstError);
    }

}