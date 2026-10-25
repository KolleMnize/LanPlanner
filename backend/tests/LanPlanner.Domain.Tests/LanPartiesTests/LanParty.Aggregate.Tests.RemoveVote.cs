using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void RemoveVote_Valid_RemovesVoteAndRaisesEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Up);

        var result = lanParty.RemoveVote(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.IsNull(lanParty.GetVote(creator, gameMode));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VoteRemoved vr && vr.GameModeId == gameMode && vr.UserId == creator && vr.RemovedDirection == VoteDirection.Up));
    }

    [TestMethod]
    public void RemoveVote_VotingClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVoteWithClosedVoting(creator, gameMode, VoteDirection.Up);

        var result = lanParty.RemoveVote(creator, gameMode);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.VotingNotOpen, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void RemoveVote_NotVoted_ReturnsNotVoted()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var result = lanParty.RemoveVote(creator, gameMode);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotVoted, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void RemoveVote_NotProposed_ReturnsGameModeNotProposed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.RemoveVote(creator, gameMode);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.GameModeNotProposed, result.Errors);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

    [TestMethod]
    public void RemoveVote_OtherUsersVoteUnchanged()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var otherUser = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Up);
        lanParty.Join(otherUser);
        lanParty.CastVote(otherUser, gameMode, VoteDirection.Down);

        var result = lanParty.RemoveVote(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.IsNull(lanParty.GetVote(creator, gameMode));
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(otherUser, gameMode));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VoteRemoved vr && vr.GameModeId == gameMode && vr.UserId == creator && vr.RemovedDirection == VoteDirection.Up));
    }

    [TestMethod]
    public void RemoveVote_ThenCastVoteAgain_Succeeds()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Up);

        var removeResult = lanParty.RemoveVote(creator, gameMode);
        var castResult = lanParty.CastVote(creator, gameMode, VoteDirection.Down);

        Assert.IsFalse(removeResult.IsError);
        Assert.IsFalse(castResult.IsError);
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(creator, gameMode));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VoteRemoved vr && vr.GameModeId == gameMode && vr.UserId == creator && vr.RemovedDirection == VoteDirection.Up));
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeDownvoted gu && gu.GameModeId == gameMode && gu.UserId == creator));
    }
}