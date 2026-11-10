using LanPlanner.Domain.Games.Events;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void Delete_Valid_RemovesAllGameModesAndRaisesEvents()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();

        // Act
        var result = game.Delete();

        // Assert
        Assert.IsFalse(result.IsError);

        Assert.HasCount(0, game.GameModes);
        Assert.IsTrue(game.DomainEvents[0] is GameModeRemoved gmr && gmr.GameId == game.Id);
        Assert.IsTrue(game.DomainEvents[1] is GameModeRemoved gmr2 && gmr2.GameId == game.Id);
        Assert.IsTrue(game.DomainEvents[2] is GameDeleted gd && gd.GameId == game.Id);

    }
}