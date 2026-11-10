using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void RemoveRating_Valid_RemovesRatingAndRaisesEvent()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(userId, stars);

        // Act
        var result = game.RemoveRating(userId);

        // Assert
        Assert.IsFalse(result.IsError);

        Assert.HasCount(0, game.Ratings);
        Assert.IsTrue(game.DomainEvents[0] is GameRatingRemoved grr && grr.GameId == game.Id && grr.UserId == userId);
    }


    [TestMethod]
    public void RemoveRating_ActorHasRatedAmongOthers_RemovesOnlyActorRating()
    {
        // Arrange
        var userIdfirst = UserId.Create(Guid.NewGuid()).Value;
        var userIdSecond = UserId.Create(Guid.NewGuid()).Value;
        var starsFirst = Stars.Create(4).Value;
        var starsSecond = Stars.Create(3).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(userIdfirst, starsFirst);
        game.Rate(userIdSecond, starsSecond);
        game.ClearDomainEvents();

        // Act
        var result = game.RemoveRating(userIdfirst);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.HasCount(1, game.Ratings);
        Assert.AreEqual(userIdSecond, game.Ratings.First().UserId);
        Assert.AreEqual(starsSecond, game.Ratings.First().Stars);
        Assert.IsTrue(game.DomainEvents[0] is GameRatingRemoved grr && grr.GameId == game.Id && grr.UserId == userIdfirst);
    }

    [TestMethod]
    public void RemoveRating_ThenRateAgain_Succeeds()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(userId, stars);
        game.RemoveRating(userId);

        // Act
        var result = game.Rate(userId, stars);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.HasCount(1, game.Ratings);
        Assert.AreEqual(userId, game.Ratings.First().UserId);
        Assert.AreEqual(stars, game.Ratings.First().Stars);
        Assert.IsTrue(game.DomainEvents[0] is GameRatingRemoved grr && grr.GameId == game.Id && grr.UserId == userId);
        Assert.IsTrue(game.DomainEvents[1] is GameRated gr && gr.GameId == game.Id && gr.UserId == userId);
    }

    [TestMethod]
    public void RemoveRating_ActorHasNotRated_ReturnsActorRatingNotFound()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var game = GameAggregateTestData.CreateMockedGame();

        // Act
        var result = game.RemoveRating(userId);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.ActorRatingNotFound);
    }
}