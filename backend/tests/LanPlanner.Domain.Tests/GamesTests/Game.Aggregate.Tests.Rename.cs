using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void Rename_Valid_UpdatesGameNameAndRaiseEvent()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var newName = "New Game Name";

        var result = game.Rename(newName);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(newName, game.Name.Value);
        Assert.IsTrue(game.DomainEvents[0] is GameRenamed gre && gre.GameId == game.Id && gre.Name == newName);
    }

    [TestMethod]
    public void Rename_SameNameButUppercase_UpdatesGameNameAndRaiseEvent()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var originalName = game.Name.Value;
        var newName = originalName.ToUpper();

        var result = game.Rename(newName);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(newName, game.Name.Value);
        Assert.IsTrue(game.DomainEvents[0] is GameRenamed gre && gre.GameId == game.Id && gre.Name == newName);
    }

    [TestMethod]
    public void Rename_SameName_ReturnsNameUnchanged()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var originalName = game.Name.Value;

        var result = game.Rename(originalName);

        Assert.IsTrue(result.IsError);

        GameAssert.FailedWith(result, game, GameErrors.GameNameUnchanged);
    }

    [TestMethod]
    public void Rename_SameNameWithSpaces_ReturnsNameUnchanged()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var originalName = game.Name.Value;
        var newName = " " + originalName + " ";

        var result = game.Rename(newName);

        Assert.IsTrue(result.IsError);

        GameAssert.FailedWith(result, game, GameErrors.GameNameUnchanged);
    }

    [TestMethod]
    public void Rename_NameIsInvalid_ReturnsNameUnchanged()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        string newName = "";

        var result = game.Rename(newName);

        Assert.IsTrue(result.IsError);

        GameAssert.FailedWith(result, game, GameErrors.NameRequired);
    }
}
