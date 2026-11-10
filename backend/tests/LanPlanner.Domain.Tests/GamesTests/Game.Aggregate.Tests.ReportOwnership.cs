using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void ReportOwnership_Valid_ReportsOwnershipAndRaisesEvent()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var userId = UserId.Create(Guid.NewGuid()).Value;

        var result = game.ReportOwnership(userId);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(1, game.Owners);
        Assert.IsTrue(game.Owners.Contains(userId));
        Assert.IsTrue(game.DomainEvents[0] is GameOwnershipReported gma && gma.GameId == game.Id && gma.UserId == userId);
    }

    [TestMethod]
    public void ReportOwnership_SecondUser_ReportsOwnershipAndRaisesEvent()
    {
        var game = GameAggregateTestData.CreateMockedGame();
        var firstUserId = UserId.Create(Guid.NewGuid()).Value;
        var secondUserId = UserId.Create(Guid.NewGuid()).Value;

        game.ReportOwnership(firstUserId);
        var result = game.ReportOwnership(secondUserId);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(2, game.Owners);
        Assert.IsTrue(game.Owners.Contains(firstUserId));
        Assert.IsTrue(game.Owners.Contains(secondUserId));
        Assert.IsTrue(game.DomainEvents[1] is GameOwnershipReported gma && gma.GameId == game.Id && gma.UserId == secondUserId);
    }


    [TestMethod]
    public void ReportOwnership_AlreadyReported_ReturnsActorOwnershipAlreadyReported()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var game = GameAggregateTestData.CreateMockedGameWithOwnership(userId);

        var result = game.ReportOwnership(userId);

        GameAssert.FailedWith(result, game, GameErrors.ActorOwnershipAlreadyReported);
    }
}