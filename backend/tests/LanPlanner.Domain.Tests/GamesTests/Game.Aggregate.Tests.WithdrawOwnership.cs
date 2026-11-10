using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void WithdrawOwnership_Valid_WithdrawsOwnershipAndRaisesEvent()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var game = GameAggregateTestData.CreateMockedGameWithOwnership(userId);

        var result = game.WithdrawOwnership(userId);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(game.Owners.Contains(userId));
        Assert.IsTrue(game.DomainEvents[0] is GameOwnershipWithdrawn gow && gow.GameId == game.Id && gow.UserId == userId);
    }

    [TestMethod]
    public void WithdrawOwnership_TwoOwners_OnlyActorIsRemoved()
    {
        var firstUserId = UserId.Create(Guid.NewGuid()).Value;
        var secondUserId = UserId.Create(Guid.NewGuid()).Value;
        var game = GameAggregateTestData.CreateMockedGame();
        game.ReportOwnership(firstUserId);
        game.ReportOwnership(secondUserId);
        game.ClearDomainEvents();

        var result = game.WithdrawOwnership(firstUserId);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(game.Owners.Contains(firstUserId));
        Assert.IsTrue(game.Owners.Contains(secondUserId));
        Assert.IsTrue(game.DomainEvents[0] is GameOwnershipWithdrawn gow && gow.GameId == game.Id && gow.UserId == firstUserId);
    }


    [TestMethod]
    public void WithdrawOwnership_ThenReportOwnership_AllowsActorToReportAgain()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var game = GameAggregateTestData.CreateMockedGameWithOwnership(userId);

        var withdrawResult = game.WithdrawOwnership(userId);
        var reportResult = game.ReportOwnership(userId);

        Assert.IsFalse(withdrawResult.IsError);
        Assert.IsFalse(reportResult.IsError);
        Assert.IsTrue(game.Owners.Contains(userId));
        Assert.IsTrue(game.DomainEvents[0] is GameOwnershipWithdrawn gow && gow.GameId == game.Id && gow.UserId == userId);
        Assert.IsTrue(game.DomainEvents[1] is GameOwnershipReported gor && gor.GameId == game.Id && gor.UserId == userId);
    }


    [TestMethod]
    public void WithdrawOwnership_NotReported_ReturnsActorOwnershipNotFound()
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var game = GameAggregateTestData.CreateMockedGame();

        var result = game.WithdrawOwnership(userId);

        GameAssert.FailedWith(result, game, GameErrors.ActorOwnershipNotFound);
    }
}