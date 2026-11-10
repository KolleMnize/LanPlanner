using ErrorOr;
using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void Rate_Valid_RatesGameNameAndRaisesEvent()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;

        var result = game.Rate(userId, stars);

        Assert.IsFalse(result.IsError);

        Assert.HasCount(1, game.Ratings);
        Assert.AreEqual(userId, game.Ratings.First(r => r.UserId == userId).UserId);
        Assert.AreEqual(stars, game.Ratings.First(r => r.UserId == userId).Stars);
        Assert.IsTrue(game.DomainEvents[0] is GameRated gma && gma.GameId == game.Id && gma.UserId == userId && gma.Stars == stars.Value);
    }

    [TestMethod]
    public void Rate_ValidForSecondRating_RatesGameNameAndRaisesEventTwice()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var userIdFirst = UserId.Create(Guid.NewGuid()).Value;
        var userIdSecond = UserId.Create(Guid.NewGuid()).Value;
        var starsFirst = Stars.Create(3).Value;
        var starsSecond = Stars.Create(4).Value;

        var result = game.Rate(userIdFirst, starsFirst);
        var resultSecond = game.Rate(userIdSecond, starsSecond);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(resultSecond.IsError);

        Assert.HasCount(2, game.Ratings);
        Assert.AreEqual(userIdFirst, game.Ratings.First(r => r.UserId == userIdFirst).UserId);
        Assert.AreEqual(starsFirst, game.Ratings.First(r => r.UserId == userIdFirst).Stars);
        Assert.AreEqual(userIdSecond, game.Ratings.First(r => r.UserId == userIdSecond).UserId);
        Assert.AreEqual(starsSecond, game.Ratings.First(r => r.UserId == userIdSecond).Stars);
        Assert.IsTrue(game.DomainEvents[0] is GameRated gma1 && gma1.GameId == game.Id && gma1.UserId == userIdFirst && gma1.Stars == starsFirst.Value);
        Assert.IsTrue(game.DomainEvents[1] is GameRated gma2 && gma2.GameId == game.Id && gma2.UserId == userIdSecond && gma2.Stars == starsSecond.Value);
    }

    [TestMethod]
    public void Rate_AlreadyRatedByUser_ReturnsActorAlreadyRated()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var stars = Stars.Create(4).Value;
        var result = game.Rate(userId, stars);
        game.ClearDomainEvents();

        var resultSecond = game.Rate(userId, stars);

        GameAssert.FailedWith(resultSecond, game, GameErrors.ActorAlreadyRated);
    }
}