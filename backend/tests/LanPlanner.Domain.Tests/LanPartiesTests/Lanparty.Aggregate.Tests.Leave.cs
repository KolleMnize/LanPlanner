using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void Leave_ParticipantWithoutVotes_RemovesParticipantAndRaisesOnlyParticipantLeft()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.Leave(participant);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.Participants.Contains(participant));
        Assert.HasCount(1, lanParty.DomainEvents);
        Assert.IsTrue(lanParty.DomainEvents[0] is ParticipantLeft left && left.UserId == participant);
    }

    [TestMethod]
    public void Leave_WithVotesAndVeto_RaisesParticipantLeftThenVoteAndVetoRemoved()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.Propose(creator, gameMode);
        lanParty.CastVote(participant, gameMode, VoteDirection.Down);
        lanParty.CastVeto(participant, gameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.Leave(participant);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(3, lanParty.DomainEvents);
        Assert.IsTrue(lanParty.DomainEvents[0] is ParticipantLeft left && left.UserId == participant);
        Assert.IsTrue(lanParty.DomainEvents[1] is VoteRemoved voteRemoved
            && voteRemoved.UserId == participant
            && voteRemoved.GameModeId == gameMode
            && voteRemoved.RemovedDirection == VoteDirection.Down);
        Assert.IsTrue(lanParty.DomainEvents[2] is VetoRemoved vetoRemoved
            && vetoRemoved.UserId == participant
            && vetoRemoved.GameModeId == gameMode);
    }

    [TestMethod]
    public void Leave_Valid_OtherParticipantsVotesAndVetosUntouched()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participantLeaving = UserId.Create(Guid.NewGuid()).Value;
        var otherParticipant = UserId.Create(Guid.NewGuid()).Value;
        var firstGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var secondGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participantLeaving);
        lanParty.Join(otherParticipant);
        lanParty.Propose(creator, firstGameMode);
        lanParty.Propose(creator, secondGameMode);
        lanParty.CastVote(participantLeaving, firstGameMode, VoteDirection.Up);
        lanParty.CastVote(otherParticipant, firstGameMode, VoteDirection.Down);
        lanParty.CastVeto(participantLeaving, secondGameMode);
        lanParty.CastVeto(otherParticipant, firstGameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.Leave(participantLeaving);

        Assert.IsFalse(result.IsError);
        Assert.IsNull(lanParty.GetVote(participantLeaving, firstGameMode));
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(otherParticipant, firstGameMode));
        Assert.IsNull(lanParty.GetVeto(participantLeaving));
        Assert.AreEqual(firstGameMode, lanParty.GetVeto(otherParticipant));
    }

    [TestMethod]
    public void Leave_Valid_ProposalsStay()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var proposer = UserId.Create(Guid.NewGuid()).Value;
        var creatorProposal = GameModeId.Create(Guid.NewGuid()).Value;
        var participantProposal = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, proposer);
        lanParty.Propose(creator, creatorProposal);
        lanParty.Propose(proposer, participantProposal);

        var result = lanParty.Leave(proposer);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.Proposals.Contains(creatorProposal));
        Assert.IsTrue(lanParty.Proposals.Contains(participantProposal));
    }

    [TestMethod]
    public void Leave_ModeratorNotLast_SucceedsAndIsNoLongerModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var secondModerator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithTwoModerators(creator, secondModerator);

        var result = lanParty.Leave(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.Participants.Contains(creator));
        Assert.IsFalse(lanParty.Moderators.Contains(creator));
        Assert.IsTrue(lanParty.Moderators.Contains(secondModerator));
    }

    [TestMethod]
    public void Leave_VotingClosed_SucceedsAndRemovesVotesAndVetos()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.Propose(creator, gameMode);
        lanParty.CastVote(participant, gameMode, VoteDirection.Up);
        lanParty.CastVeto(participant, gameMode);
        lanParty.CloseVoting(creator);
        lanParty.ClearDomainEvents();

        var result = lanParty.Leave(participant);

        Assert.IsFalse(result.IsError);
        Assert.IsNull(lanParty.GetVote(participant, gameMode));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is ParticipantLeft participantLeft
            && participantLeft.UserId == participant));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is VoteRemoved voteRemoved
            && voteRemoved.UserId == participant
            && voteRemoved.GameModeId == gameMode
            && voteRemoved.RemovedDirection == VoteDirection.Up));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is VetoRemoved vetoRemoved
            && vetoRemoved.UserId == participant
            && vetoRemoved.GameModeId == gameMode));
    }

    [TestMethod]
    public void Leave_ThenJoinAgain_IsParticipantWithoutVotes()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.Propose(creator, gameMode);
        lanParty.CastVote(participant, gameMode, VoteDirection.Up);

        var leaveResult = lanParty.Leave(participant);
        var joinResult = lanParty.Join(participant);

        Assert.IsFalse(leaveResult.IsError);
        Assert.IsFalse(joinResult.IsError);
        Assert.IsTrue(lanParty.Participants.Contains(participant));
        Assert.IsNull(lanParty.GetVote(participant, gameMode));
    }

    [TestMethod]
    public void Leave_NotParticipant_ReturnsNotParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var nonParticipant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.Leave(nonParticipant);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAParticipant);
    }

    [TestMethod]
    public void Leave_Ended_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.CloseVoting(creator);
        lanParty.EndParty(creator);
        lanParty.ClearDomainEvents();

        var result = lanParty.Leave(participant);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.Ended);
    }

    [TestMethod]
    public void Leave_LastModerator_ReturnsLastModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.Leave(creator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.LastModerator);
    }
}
