using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void CastVeto_Valid_AddsVetoAndRaisesEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var result = lanParty.CastVeto(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.IsNotNull(lanParty.GetVeto(creator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is GameModeVetoed vetoed && vetoed.GameModeId == gameMode && vetoed.UserId == creator));
    }

    [TestMethod]
    public void CastVeto_NotParticipant_ReturnsNotParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);
        var nonParticipant = UserId.Create(Guid.NewGuid()).Value;

        var result = lanParty.CastVeto(nonParticipant, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAParticipant);
    }

    [TestMethod]
    public void CastVeto_VotingClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVoteWithClosedVoting(creator, gameMode, VoteDirection.Up);

        var result = lanParty.CastVeto(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotOpen);
    }

    [TestMethod]
    public void CastVeto_NotProposed_ReturnsGameModeNotProposed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.CastVeto(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.GameModeNotProposed);
    }

    [TestMethod]
    public void CastVeto_AlreadyVetoedSameMode_ReturnsAlreadyVetoed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);
        lanParty.CastVeto(creator, gameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.CastVeto(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.AlreadyVetoed);
    }

    [TestMethod]
    public void CastVeto_AlreadyVetoedOtherMode_ReturnsAlreadyVetoed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var firstGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var secondGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, firstGameMode);
        lanParty.Propose(creator, secondGameMode);
        lanParty.CastVeto(creator, firstGameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.CastVeto(creator, secondGameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.AlreadyVetoed);
    }

    [TestMethod]
    public void CastVeto_OtherUserSameMode_Succeeds()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.Propose(creator, gameMode);
        lanParty.ClearDomainEvents();

        var creatorResult = lanParty.CastVeto(creator, gameMode);
        var participantResult = lanParty.CastVeto(participant, gameMode);

        Assert.IsFalse(creatorResult.IsError);
        Assert.IsFalse(participantResult.IsError);
        Assert.IsNotNull(lanParty.GetVeto(creator));
        Assert.IsNotNull(lanParty.GetVeto(participant));
        Assert.AreEqual(2, lanParty.DomainEvents.Count(e => e is GameModeVetoed vetoed && vetoed.GameModeId == gameMode));
    }

    [TestMethod]
    public void CastVeto_WithExistingVote_Succeeds()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Up);
        lanParty.ClearDomainEvents();

        var result = lanParty.CastVeto(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(VoteDirection.Up, lanParty.GetVote(creator, gameMode));
        Assert.IsNotNull(lanParty.GetVeto(creator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is GameModeVetoed vetoed && vetoed.GameModeId == gameMode && vetoed.UserId == creator));
    }

}