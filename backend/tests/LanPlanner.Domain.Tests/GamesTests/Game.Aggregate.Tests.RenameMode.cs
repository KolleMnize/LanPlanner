using LanPlanner.Domain.Games;
using LanPlanner.Domain.Games.Events;

namespace LanPlanner.Domain.Tests.GamesTests;

public partial class GameAggregateTests
{
    [TestMethod]
    public void RenameMode_Valid_RenamesGameModeAndRaisesEvent()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var secondMode = game.GameModes.Last();
        var newName = GameModeName.Create("Renamed Mode").Value;

        // Act
        var result = game.RenameMode(secondMode.Id, newName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(game.GameModes.First(x => x.Id == secondMode.Id).Name == newName);
        Assert.IsTrue(game.DomainEvents[0] is GameModeRenamed gmr
            && gmr.GameId == game.Id
            && gmr.GameModeId == secondMode.Id
            && gmr.GameModeName == newName.Value);
    }

    [TestMethod]
    public void RenameMode_SameNameButUppercase_RenamesGameModeAndRaisesEvent()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var secondMode = game.GameModes.Last();
        var newName = GameModeName.Create(secondMode.Name.Value.ToUpper()).Value;

        // Act
        var result = game.RenameMode(secondMode.Id, newName);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(game.GameModes.First(x => x.Id == secondMode.Id).Name == newName);
        Assert.IsTrue(game.DomainEvents[0] is GameModeRenamed gmr
            && gmr.GameId == game.Id
            && gmr.GameModeId == secondMode.Id
            && gmr.GameModeName == newName.Value);
    }

    [TestMethod]
    public void RenameMode_GameModeNotExisting_ReturnsGameModeNotFound()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var nonExistingModeId = GameModeId.Create(Guid.NewGuid()).Value;
        var newName = GameModeName.Create("Non Existing Mode").Value;

        // Act
        var result = game.RenameMode(nonExistingModeId, newName);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.GameModeNotFound);
    }

    [TestMethod]
    public void RenameMode_GameModeIsDefault_ReturnsDefaultModeCannotBeRenamed()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var defaultMode = game.GameModes.First();
        var newName = GameModeName.Create("New Default Name").Value;

        // Act
        var result = game.RenameMode(defaultMode.Id, newName);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.DefaultModeCannotBeRenamed);
    }

    [TestMethod]
    public void RenameMode_SameName_ReturnsGameModeNameUnchanged()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var secondMode = game.GameModes.Last();
        var newName = secondMode.Name;

        // Act
        var result = game.RenameMode(secondMode.Id, newName);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.GameModeNameUnchanged);
    }

    [TestMethod]
    public void RenameMode_NameAlreadyTaken_ReturnsGameModeNameAlreadyTaken()
    {
        // Arrange
        var game = GameAggregateTestData.CreateMockedGameWithSecondGameMode();
        var secondMode = game.GameModes.Last();
        var firstMode = game.GameModes.First();
        var newName = firstMode.Name;

        // Act
        var result = game.RenameMode(secondMode.Id, newName);

        // Assert
        GameAssert.FailedWith(result, game, GameErrors.GameModeNameAlreadyTaken);
    }
}
