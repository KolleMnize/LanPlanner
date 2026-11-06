using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void ChangeMaxPlayers_Valid_ChangesMaxPlayersAndRaisesEvent()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var secondMode = game.GameModes.Last();
        var newMaxPlayers = MaxPlayers.Create(16).Value;

        // Act
        var result = game.ChangeMaxPlayers(secondMode.Id, newMaxPlayers);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(game.GameModes.First(x => x.Id == secondMode.Id).MaxPlayers == newMaxPlayers);
        Assert.IsTrue(game.DomainEvents[0] is GameModeMaxPlayersChanged gmpc
            && gmpc.GameId == game.Id
            && gmpc.GameModeId == secondMode.Id
            && gmpc.MaxPlayers == newMaxPlayers.Value);
    }

    [TestMethod]
    public void ChangeMaxPlayers_OnDefaultGameMode_ChangesMaxPlayersAndRaisesEvent()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var defaultMode = game.GameModes.First();
        var newMaxPlayers = MaxPlayers.Create(16).Value;

        // Act
        var result = game.ChangeMaxPlayers(defaultMode.Id, newMaxPlayers);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(defaultMode.IsDefault);
        Assert.IsTrue(game.GameModes.First(x => x.Id == defaultMode.Id).MaxPlayers == newMaxPlayers);
        Assert.IsTrue(game.DomainEvents[0] is GameModeMaxPlayersChanged gmpc
            && gmpc.GameId == game.Id
            && gmpc.GameModeId == defaultMode.Id
            && gmpc.MaxPlayers == newMaxPlayers.Value);
    }

    [TestMethod]
    public void ChangeMaxPlayers_GameModeNotExisting_ReturnsGameModeNotFound()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var nonExistingModeId = GameModeId.Create(Guid.NewGuid()).Value;
        var newMaxPlayers = MaxPlayers.Create(16).Value;

        // Act
        var result = game.ChangeMaxPlayers(nonExistingModeId, newMaxPlayers);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.GameModeNotFound);
    }

    [TestMethod]
    public void ChangeMaxPlayers_SameMaxPlayers_ReturnsGameModeMaxPlayersUnchanged()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var secondMode = game.GameModes.Last();
        var currentMaxPlayers = secondMode.MaxPlayers;

        // Act
        var result = game.ChangeMaxPlayers(secondMode.Id, currentMaxPlayers);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.GameModeMaxPlayersUnchanged);
    }
}