using ErrorOr;
using LanPlanner.Domain.Games;

namespace LanPlanner.Domain.Tests.GamesTests;

public static class GameAssert
{
    public static void FailedWith(ErrorOr<Success> result, Game game, Error expected)
    {
        Assert.IsTrue(result.IsError, "Expected an error, but the command succeeded.");
        Assert.AreEqual(expected.Code, result.FirstError.Code);
        Assert.IsEmpty(game.DomainEvents, "No events expected on failure.");
    }
}
