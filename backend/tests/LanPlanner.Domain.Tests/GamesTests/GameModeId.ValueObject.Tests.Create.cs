using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public partial class GameModeIdValueObjectTests
{
    [TestMethod]
    public void Create_ValidGuid_ReturnsGameModeId()
    {
        var guid = Guid.NewGuid();

        var result = GameModeId.Create(guid);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(guid, result.Value.Value);
    }

    [TestMethod]
    public void Create_InvalidGuid_ReturnsError()
    {
        var guid = Guid.Empty;

        var result = GameModeId.Create(guid);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.GameModeIdCannotBeEmpty, result.FirstError);
    }
}
