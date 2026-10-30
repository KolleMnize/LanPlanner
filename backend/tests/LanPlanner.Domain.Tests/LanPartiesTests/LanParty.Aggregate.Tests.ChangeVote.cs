using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void ChangeVote_FromUp_ChangesToDownAndRaisesEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Up);

        var result = lanParty.ChangeVote(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(creator, gameMode));
        Assert.IsTrue(lanParty.DomainEvents[0] is VoteChanged vc && vc.GameModeId == gameMode && vc.UserId == creator && vc.NewDirection == VoteDirection.Down);
    }

    [TestMethod]
    public void ChangeVote_FromDown_ChangesToUpAndRaisesEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Down);

        var result = lanParty.ChangeVote(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(VoteDirection.Up, lanParty.GetVote(creator, gameMode));
        Assert.IsTrue(lanParty.DomainEvents[0] is VoteChanged vc && vc.GameModeId == gameMode && vc.UserId == creator && vc.NewDirection == VoteDirection.Up);
    }

    [TestMethod]
    public void ChangeVote_VotingClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVoteWithClosedVoting(creator, gameMode, VoteDirection.Up);

        var result = lanParty.ChangeVote(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotOpen);
    }

    [TestMethod]
    public void ChangeVote_NotVoted_ReturnsNotVoted()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(creator, gameMode);

        var result = lanParty.ChangeVote(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotVoted);
    }

    [TestMethod]
    public void ChangeVote_NotProposed_ReturnsGameModeNotProposed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.ChangeVote(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.GameModeNotProposed);
    }

    [TestMethod]
    public void ChangeVote_OtherUsersVoteUnchanged()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var otherUser = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalAndVote(creator, gameMode, VoteDirection.Up);
        lanParty.Join(otherUser);
        lanParty.CastVote(otherUser, gameMode, VoteDirection.Up);
        lanParty.ClearDomainEvents();

        var result = lanParty.ChangeVote(creator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(creator, gameMode));
        Assert.AreEqual(VoteDirection.Up, lanParty.GetVote(otherUser, gameMode));
        Assert.IsTrue(lanParty.DomainEvents[0] is VoteChanged vc && vc.UserId == creator && vc.NewDirection == VoteDirection.Down && vc.GameModeId == gameMode);
    }
}
