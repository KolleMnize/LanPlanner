using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void OpenVoting_VotingClosed_OpensVoting()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.OpenVoting(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.VotingIsOpen);
    }

    [TestMethod]
    public void OpenVoting_VotingClosed_RaisesVotingOpened()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        lanParty.OpenVoting(creator);

        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is VotingOpened));
    }

    [TestMethod]
    public void OpenVoting_NotModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var notModerator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.OpenVoting(notModerator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAModerator);
    }

    [TestMethod]
    public void OpenVoting_AlreadyOpen_ReturnsVotingNotClosed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.OpenVoting(creator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotClosed);
    }

    [TestMethod]
    public void OpenVoting_Ended_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyEnded(creator);

        var result = lanParty.OpenVoting(creator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.Ended);
    }
}