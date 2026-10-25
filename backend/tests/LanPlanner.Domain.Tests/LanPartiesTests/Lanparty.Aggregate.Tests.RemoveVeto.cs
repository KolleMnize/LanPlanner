using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void RemoveVeto_Valid_RemovesVetoAndRaisesEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);
        lanParty.CastVeto(creator, gameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.RemoveVeto(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsNull(lanParty.GetVeto(creator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VetoRemoved removed && removed.GameModeId == gameMode && removed.UserId == creator));
    }

    [TestMethod]
    public void RemoveVeto_VotingClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);
        lanParty.CastVeto(creator, gameMode);
        lanParty.CloseVoting(creator);
        lanParty.ClearDomainEvents();

        var result = lanParty.RemoveVeto(creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.VotingNotOpen, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void RemoveVeto_NotVetoed_ReturnsNotVetoed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var result = lanParty.RemoveVeto(creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotVetoed, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void RemoveVeto_OtherUsersVetoUnchanged()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var otherUser = UserId.Create(Guid.NewGuid()).Value;
        var firstGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var secondGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, otherUser);
        lanParty.Propose(creator, firstGameMode);
        lanParty.Propose(creator, secondGameMode);
        lanParty.CastVeto(creator, firstGameMode);
        lanParty.CastVeto(otherUser, secondGameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.RemoveVeto(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsNull(lanParty.GetVeto(creator));
        Assert.AreEqual(secondGameMode, lanParty.GetVeto(otherUser));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VetoRemoved removed && removed.GameModeId == firstGameMode && removed.UserId == creator));
    }

    [TestMethod]
    public void RemoveVeto_ThenCastVetoOnOtherMode_Succeeds()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var firstGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var secondGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, firstGameMode);
        lanParty.Propose(creator, secondGameMode);
        lanParty.CastVeto(creator, firstGameMode);
        lanParty.ClearDomainEvents();

        var removeResult = lanParty.RemoveVeto(creator);
        var castResult = lanParty.CastVeto(creator, secondGameMode);

        Assert.IsFalse(removeResult.IsError);
        Assert.IsFalse(castResult.IsError);
        Assert.AreEqual(secondGameMode, lanParty.GetVeto(creator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VetoRemoved removed && removed.GameModeId == firstGameMode && removed.UserId == creator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeVetoed vetoed && vetoed.GameModeId == secondGameMode && vetoed.UserId == creator));
    }
}
