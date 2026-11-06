using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void AddMode_Valid_AddsModeAndRaisesEvent()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        string newModeName = "New Mode";
        int maxPlayers = 4;

        var result = game.AddMode(GameModeName.Create(newModeName).Value, MaxPlayers.Create(maxPlayers).Value);

        Assert.IsFalse(result.IsError);

        Assert.AreEqual(newModeName, game.GameModes.First(m => m.Name.Value == newModeName).Name.Value);
        Assert.AreEqual(maxPlayers, game.GameModes.First(m => m.Name.Value == newModeName).MaxPlayers.Value);
        Assert.IsTrue(game.DomainEvents[0] is GameModeAdded gma && gma.GameId == game.Id && gma.GameModeName == newModeName);
    }

    [TestMethod]
    public void AddMode_NameAlreadyTaken_ReturnsGameModeNameAlreadyTaken()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        string existingModeName = game.GameModes.First().Name.Value;
        int maxPlayers = 4;

        var result = game.AddMode(GameModeName.Create(existingModeName).Value, MaxPlayers.Create(maxPlayers).Value);

        GameAssert.FailedWith(result, game, GameErrors.GameModeNameAlreadyTaken);
    }
}