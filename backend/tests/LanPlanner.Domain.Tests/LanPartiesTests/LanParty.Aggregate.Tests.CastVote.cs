
using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void Vote_Up_Valid_AddsUpvoteAndRaisesGameModeUpvoted()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var result = lanParty.CastVote(creator, gameMode, VoteDirection.Up);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(VoteDirection.Up, lanParty.GetVote(creator, gameMode));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeUpvoted gmuv && gmuv.GameModeId == gameMode && gmuv.UserId == creator));
    }

    [TestMethod]
    public void Vote_Down_Valid_AddsDownvoteAndRaisesGameModeDownvoted()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var result = lanParty.CastVote(creator, gameMode, VoteDirection.Down);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(creator, gameMode));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeDownvoted gmdv && gmdv.GameModeId == gameMode && gmdv.UserId == creator));
    }

    [TestMethod]
    public void Vote_NotParticipant_ReturnsNotParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var nonParticipant = UserId.Create(Guid.NewGuid()).Value;
        var result = lanParty.CastVote(nonParticipant, gameMode, VoteDirection.Up);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotAParticipant, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void Vote_VotingClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalWhichEnded(creator, gameMode);

        var result = lanParty.CastVote(creator, gameMode, VoteDirection.Up);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.VotingNotOpen, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }
    [TestMethod]
    public void Vote_NotProposed_ReturnsGameModeNotProposed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var nonProposedGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var result = lanParty.CastVote(creator, nonProposedGameMode, VoteDirection.Up);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.GameModeNotProposed, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void Vote_SameUserOtherGameMode_Succeeds()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode1 = GameModeId.Create(Guid.NewGuid()).Value;
        var gameMode2 = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode1);
        lanParty.Propose(creator, gameMode2);

        var result = lanParty.CastVote(creator, gameMode2, VoteDirection.Up);
        var result2 = lanParty.CastVote(creator, gameMode1, VoteDirection.Down);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(result2.IsError);
        Assert.AreEqual(VoteDirection.Up, lanParty.GetVote(creator, gameMode2));
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(creator, gameMode1));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeUpvoted gmuv && gmuv.GameModeId == gameMode2 && gmuv.UserId == creator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeDownvoted gmdv && gmdv.GameModeId == gameMode1 && gmdv.UserId == creator));
    }

}