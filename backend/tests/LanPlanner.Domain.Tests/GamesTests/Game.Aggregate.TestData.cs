using LanPlanner.Domain.Games;

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
}
