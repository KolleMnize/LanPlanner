using LanPlanner.Domain.Games;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.GamesTests;

internal static class GameAggregateTestData
{
    internal static Game CreateMockedGame()
    {
        var gameId = GameId.Create(Guid.NewGuid()).Value;
        var gameName = GameName.Create("Test Game").Value;
        var maxPlayers = MaxPlayers.Create(4).Value;

        var game = Game.Create(gameId, gameName, maxPlayers).Value;
        game.ClearDomainEvents();
        return game;
    }

    internal static Game CreateMockedGameWithSecondGameMode()
    {
        var gameId = GameId.Create(Guid.NewGuid()).Value;
        var gameName = GameName.Create("Test Game").Value;
        var maxPlayers = MaxPlayers.Create(4).Value;

        var game = Game.Create(gameId, gameName, maxPlayers).Value;
        game.AddMode(GameModeName.Create("Second Mode").Value, maxPlayers);

        game.ClearDomainEvents();
        return game;
    }

    internal static Game CreateMockedGameWithRating(UserId userId, Stars stars)
    {
        var game = CreateMockedGame();
        game.Rate(userId, stars);
        game.ClearDomainEvents();
        return game;
    }
    internal static Game CreateMockedGameWithOwnership(UserId userId)
    {
        var game = CreateMockedGame();
        game.ReportOwnership(userId);
        game.ClearDomainEvents();
        return game;
    }
}
