using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void RemoveMode_Valid_RemovesModeAndRaisesEvent()
    {
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var modeId = game.GameModes.First(m => !m.IsDefault).Id;

        var result = game.RemoveMode(modeId);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(game.GameModes.Any(m => m.Id == modeId));
        Assert.IsTrue(game.DomainEvents[0] is GameModeRemoved gma && gma.GameId == game.Id && gma.GameModeId == modeId);
    }

    [TestMethod]
    public void RemoveMode_UnknownModeId_ReturnsGameModeNotFound()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var unknownModeId = GameModeId.Create(Guid.NewGuid()).Value;

        var result = game.RemoveMode(unknownModeId);

        GameAssert.FailedWith(result, game, GameErrors.GameModeNotFound);
    }


    [TestMethod]
    public void RemoveMode_DefaultMode_ReturnsDefaultModeCannotBeRemoved()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var defaultModeId = game.GameModes.First(m => m.IsDefault).Id;

        var result = game.RemoveMode(defaultModeId);

        GameAssert.FailedWith(result, game, GameErrors.DefaultModeCannotBeRemoved);
    }

    [TestMethod]
    public void RemoveMode_ModeNameBecomesAvailable_AddModeWithSameNameSucceeds()
    {
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var mode = game.GameModes.First(m => !m.IsDefault);
        var modeId = mode.Id;
        var modeName = mode.Name;

        var removeResult = game.RemoveMode(modeId);
        var addResult = game.AddMode(modeName, MaxPlayers.Create(4).Value);

        Assert.IsFalse(removeResult.IsError);
        Assert.IsFalse(addResult.IsError);
        Assert.IsTrue(game.DomainEvents[0] is GameModeRemoved gmr && gmr.GameId == game.Id && gmr.GameModeId == modeId);
        Assert.IsTrue(game.DomainEvents[1] is GameModeAdded gma && gma.GameId == game.Id && gma.GameModeName == modeName.Value);
    }
}