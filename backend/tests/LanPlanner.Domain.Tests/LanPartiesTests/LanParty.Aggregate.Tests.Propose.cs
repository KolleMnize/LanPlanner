using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{

    [TestMethod]
    public void Propose_Valid_AddsProposal()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;

        var result = lanParty.Propose(participant, gameMode);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.Proposals.Any(p => p == gameMode));
    }

    [TestMethod]
    public void Propose_Valid_RaisesGameModeProposed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;

        lanParty.Propose(participant, gameMode);

        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is GameModeProposed gmp && gmp.ProposedGameMode == gameMode && gmp.ProposedBy == participant));
    }

    [TestMethod]
    public void Propose_NotParticipant_ReturnsNotAParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var nonParticipant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;

        var result = lanParty.Propose(nonParticipant, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAParticipant);
    }

    [TestMethod]
    public void Propose_VotingClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;


        var result = lanParty.Propose(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotOpen);
    }

    [TestMethod]
    public void Propose_AlreadyProposed_ReturnsGameModeAlreadyProposed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);
        var gameMode = GameModeId.Create(Guid.NewGuid()).Value;
        lanParty.Propose(creator, gameMode);
        lanParty.ClearDomainEvents();

        var result = lanParty.Propose(creator, gameMode);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.GameModeAlreadyProposed);
    }
}