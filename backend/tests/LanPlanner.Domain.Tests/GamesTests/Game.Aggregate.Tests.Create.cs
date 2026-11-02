using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Tests.GamesTests;

[TestClass]
public partial class GameAggregateTests
{
    [TestMethod]
    public void Create_Valid_SetsNameAndDefaultGameMode()
    {
        var gameId = GameId.Create(Guid.NewGuid()).Value;
        var gameName = GameName.Create("Test Game").Value;
        var maxPlayers = MaxPlayers.Create(4).Value;

        var result = Game.Create(gameId, gameName, maxPlayers);

        Assert.IsFalse(result.IsError);
        var game = result.Value;
        Assert.AreEqual(gameName, game.Name);
        Assert.HasCount(1, game.GameModes);
        var defaultGameMode = game.GameModes[0];
        Assert.IsTrue(defaultGameMode.IsDefault);
        Assert.AreEqual(maxPlayers, defaultGameMode.MaxPlayers);
        Assert.AreEqual("Standard", defaultGameMode.Name.Value);
        Assert.IsNotNull(game.Id);
        Assert.AreNotEqual(Guid.Empty, game.Id.Value);
    }

    [TestMethod]
    public void Create_Valid_RaisesTwoEventsInOrder()
    {
        int maxPlayersValue = 4;

        var gameId = GameId.Create(Guid.NewGuid()).Value;
        var gameName = GameName.Create("Test Game").Value;
        var maxPlayers = MaxPlayers.Create(maxPlayersValue).Value;

        var result = Game.Create(gameId, gameName, maxPlayers);
        var gameMode = result.Value.GameModes[0];


        Assert.IsFalse(result.IsError);

        CollectionAssert.AreEqual(
            new IDomainEvent[]
            {
            new GameCreated(gameId, gameName),
            new GameModeAdded(gameId, gameMode.Id, "Standard", maxPlayersValue),
            },
            result.Value.DomainEvents.ToList());
    }
}