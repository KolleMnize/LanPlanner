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

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAModerator);
    }

    [TestMethod]
    public void CloseVoting_AlreadyClosed_ReturnsVotingNotOpen()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.CloseVoting(creator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotOpen);
    }
}
