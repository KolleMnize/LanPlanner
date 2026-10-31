using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public partial class MaxPlayersValueObjectTests
{
    [TestMethod]
    public void Create_ValidMaxPlayers_ReturnsMaxPlayers()
    {
        var maxPlayers = 4;

        var result = MaxPlayers.Create(maxPlayers);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(maxPlayers, result.Value.Value);
    }

    [TestMethod]
    public void Create_MaxPlayersZero_ReturnsError()
    {
        var maxPlayers = 0;

        var result = MaxPlayers.Create(maxPlayers);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.MaxPlayersInvalid, result.FirstError);
    }
    [TestMethod]
    public void Create_MaxPlayersTooLow_ReturnsError()
    {
        var maxPlayers = -1;

        var result = MaxPlayers.Create(maxPlayers);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.MaxPlayersInvalid, result.FirstError);
    }
}