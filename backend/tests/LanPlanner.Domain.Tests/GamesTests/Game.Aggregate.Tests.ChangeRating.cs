using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void ChangeRating_Valid_ChangesRatingAndRaisesEvent()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(userId, stars);
        var newStars = Stars.Create(5).Value;

        // Act
        var result = game.ChangeRating(userId, newStars);

        // Assert
        Assert.IsFalse(result.IsError);

        Assert.HasCount(1, game.Ratings);
        Assert.AreEqual(userId, game.Ratings.First(r => r.UserId == userId).UserId);
        Assert.AreEqual(newStars, game.Ratings.First(r => r.UserId == userId).Stars);
        Assert.IsTrue(game.DomainEvents[0] is GameRatingChanged gma && gma.GameId == game.Id && gma.UserId == userId && gma.Stars == newStars.Value);
    }


    [TestMethod]
    public void ChangeRating_TwoRaters_OnlyActorRatingChanges()
    {
        // Arrange
        var userId1 = UserId.Create(Guid.NewGuid()).Value;
        var userId2 = UserId.Create(Guid.NewGuid()).Value;
        var stars1 = Stars.Create(4).Value;
        var stars2 = Stars.Create(3).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(userId1, stars1);
        game.Rate(userId2, stars2);
        game.ClearDomainEvents();

        var newStars1 = Stars.Create(5).Value;

        // Act
        var result = game.ChangeRating(userId1, newStars1);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(newStars1, game.Ratings.First(r => r.UserId == userId1).Stars);
        Assert.AreEqual(stars2, game.Ratings.First(r => r.UserId == userId2).Stars);
        Assert.IsTrue(game.DomainEvents[0] is GameRatingChanged gma && gma.GameId == game.Id && gma.UserId == userId1 && gma.Stars == newStars1.Value);
    }

    // Arrange
    [TestMethod]
    public void ChangeRating_ActorHasNotRated_ReturnsActorRatingNotFound()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var game = GameAggregateTestData.CreateMockedGame();

        // Act
        var result = game.ChangeRating(userId, stars);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.ActorRatingNotFound);
    }

    [TestMethod]
    public void ChangeRating_ActorHasNotRatedButOthersHave_ReturnsActorRatingNotFound()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var otherUserId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var otherStars = Stars.Create(3).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(otherUserId, otherStars);

        // Act
        var result = game.ChangeRating(userId, stars);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.ActorRatingNotFound);
    }

    [TestMethod]
    public void ChangeRating_SameStars_ReturnsRatingUnchanged()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var game = GameAggregateTestData.CreateMockedGameWithRating(userId, stars);

        // Act
        var result = game.ChangeRating(userId, stars);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.RatingUnchanged);
    }

}