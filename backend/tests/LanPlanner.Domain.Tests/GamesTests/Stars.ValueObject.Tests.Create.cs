using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public partial class StarsValueObjectTests
{
    [TestMethod]
    public void Create_ValidStars_ReturnsStars()
    {
        var stars = 4;

        var result = Stars.Create(stars);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(stars, result.Value.Value);
    }

    [TestMethod]
    public void Create_StarsTooLow_ReturnsStarsOutOfRange()
    {
        var stars = 0;

        var result = Stars.Create(stars);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.StarsOutOfRange, result.FirstError);
    }
    [TestMethod]
    public void Create_StarsTooHigh_ReturnsStarsOutOfRange()
    {
        var stars = 6;

        var result = Stars.Create(stars);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(GameErrors.StarsOutOfRange, result.FirstError);
    }
}