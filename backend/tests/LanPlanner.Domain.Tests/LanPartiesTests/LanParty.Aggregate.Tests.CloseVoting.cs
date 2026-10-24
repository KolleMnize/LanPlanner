using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void CloseVoting_Moderator_ClosesVoting()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.CloseVoting(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.VotingIsOpen);
    }

    [TestMethod]
    public void CloseVoting_Moderator_RaisesVotingClosed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.CloseVoting(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VotingClosed));
    }

    [TestMethod]
    public void CloseVoting_NotAModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var nonModerator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.CloseVoting(nonModerator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotAModerator, result.Errors);
    }

    [TestMethod]
    public void CloseVoting_AlreadyClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.CloseVoting(creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.VotingNotOpen, result.Errors);
    }

    [TestMethod]
    public void CloseVoting_Invalid_KeepsStateAndRaisesNoEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.CloseVoting(creator);

        Assert.IsTrue(result.IsError);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }
}
