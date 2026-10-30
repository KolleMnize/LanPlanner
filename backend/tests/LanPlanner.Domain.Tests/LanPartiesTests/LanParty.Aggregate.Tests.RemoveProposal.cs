using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void RemoveProposal_WithoutVotesAndVetos_RemovesProposalAndRaisesOnlyProposalRemoved()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(moderator, gameMode);

        var result = lanParty.RemoveProposal(moderator, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.Proposals.Contains(gameMode));
        Assert.HasCount(1, lanParty.DomainEvents);
        Assert.IsTrue(lanParty.DomainEvents[0] is ProposalRemoved removed
            && removed.GameModeId == gameMode
            && removed.RemovedBy == moderator);
    }

    [TestMethod]
    public void RemoveProposal_WithVotesAndVetos_RaisesProposalRemovedThenVoteAndVetoRemoved()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(moderator, participant);
        lanParty.Propose(moderator, gameMode);
        lanParty.CastVote(moderator, gameMode, VoteDirection.Up);
        lanParty.CastVote(participant, gameMode, VoteDirection.Down);
        lanParty.CastVeto(moderator, gameMode);
        lanParty.CastVeto(participant, gameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.RemoveProposal(moderator, gameMode);
        var events = lanParty.DomainEvents;

        Assert.IsFalse(result.IsError);
        Assert.HasCount(5, events);
        Assert.IsTrue(events[0] is ProposalRemoved proposalRemoved
            && proposalRemoved.GameModeId == gameMode
            && proposalRemoved.RemovedBy == moderator);
        Assert.IsTrue(events[1] is VoteRemoved firstVoteRemoved
            && firstVoteRemoved.GameModeId == gameMode
            && firstVoteRemoved.UserId == moderator
            && firstVoteRemoved.RemovedDirection == VoteDirection.Up);
        Assert.IsTrue(events[2] is VoteRemoved secondVoteRemoved
            && secondVoteRemoved.GameModeId == gameMode
            && secondVoteRemoved.UserId == participant
            && secondVoteRemoved.RemovedDirection == VoteDirection.Down);
        Assert.IsTrue(events[3] is VetoRemoved firstVetoRemoved
            && firstVetoRemoved.GameModeId == gameMode
            && firstVetoRemoved.UserId == moderator);
        Assert.IsTrue(events[4] is VetoRemoved secondVetoRemoved
            && secondVetoRemoved.GameModeId == gameMode
            && secondVetoRemoved.UserId == participant);
    }

    [TestMethod]
    public void RemoveProposal_Valid_OtherProposalsUntouched()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var otherParticipant = UserId.Create(Guid.NewGuid()).Value;
        var removedGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var remainingGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(moderator, participant);
        lanParty.Join(otherParticipant);
        lanParty.Propose(moderator, removedGameMode);
        lanParty.Propose(moderator, remainingGameMode);
        lanParty.CastVote(moderator, removedGameMode, VoteDirection.Up);
        lanParty.CastVote(participant, remainingGameMode, VoteDirection.Down);
        lanParty.CastVote(otherParticipant, remainingGameMode, VoteDirection.Up);
        lanParty.CastVeto(moderator, removedGameMode);
        lanParty.CastVeto(participant, remainingGameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.RemoveProposal(moderator, removedGameMode);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.Proposals.Contains(removedGameMode));
        Assert.IsTrue(lanParty.Proposals.Contains(remainingGameMode));
        Assert.IsNull(lanParty.GetVote(moderator, removedGameMode));
        Assert.AreEqual(VoteDirection.Down, lanParty.GetVote(participant, remainingGameMode));
        Assert.AreEqual(VoteDirection.Up, lanParty.GetVote(otherParticipant, remainingGameMode));
        Assert.IsNull(lanParty.GetVeto(moderator));
        Assert.AreEqual(remainingGameMode, lanParty.GetVeto(participant));
        Assert.HasCount(3, lanParty.DomainEvents);
        Assert.IsTrue(lanParty.DomainEvents[0] is ProposalRemoved removed
            && removed.GameModeId == removedGameMode);
        Assert.IsTrue(lanParty.DomainEvents[1] is VoteRemoved voteRemoved
            && voteRemoved.GameModeId == removedGameMode
            && voteRemoved.UserId == moderator
            && voteRemoved.RemovedDirection == VoteDirection.Up);
        Assert.IsTrue(lanParty.DomainEvents[2] is VetoRemoved vetoRemoved
            && vetoRemoved.GameModeId == removedGameMode
            && vetoRemoved.UserId == moderator);
    }

    [TestMethod]
    public void RemoveProposal_Valid_UserCanCastVetoAgain()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var removedGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var remainingGameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposal(moderator, removedGameMode);
        lanParty.Propose(moderator, remainingGameMode);
        lanParty.CastVeto(moderator, removedGameMode);
        lanParty.ClearDomainEvents();

        var removeResult = lanParty.RemoveProposal(moderator, removedGameMode);
        var castResult = lanParty.CastVeto(moderator, remainingGameMode);

        Assert.IsFalse(removeResult.IsError);
        Assert.IsFalse(castResult.IsError);
        Assert.AreEqual(remainingGameMode, lanParty.GetVeto(moderator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is VetoRemoved removed && removed.GameModeId == removedGameMode && removed.UserId == moderator));
        Assert.IsTrue(lanParty.DomainEvents.Any(e =>
            e is GameModeVetoed vetoed && vetoed.GameModeId == remainingGameMode && vetoed.UserId == moderator));
    }

    [TestMethod]
    public void RemoveProposal_ParticipantNotModerator_ReturnsNotModerator()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(moderator, participant);
        lanParty.Propose(moderator, gameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.RemoveProposal(participant, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAModerator);
        Assert.IsTrue(lanParty.Proposals.Contains(gameMode));
    }

    [TestMethod]
    public void RemoveProposal_VotingClosed_ReturnsVotingNotOpen()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithProposalWhichEnded(moderator, gameMode);

        var result = lanParty.RemoveProposal(moderator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotOpen);
        Assert.IsTrue(lanParty.Proposals.Contains(gameMode));
    }

    [TestMethod]
    public void RemoveProposal_NotProposed_ReturnsGameModeNotProposed()
    {
        var moderator = UserId.Create(Guid.NewGuid()).Value;
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(moderator);

        var result = lanParty.RemoveProposal(moderator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.GameModeNotProposed);
    }
}
