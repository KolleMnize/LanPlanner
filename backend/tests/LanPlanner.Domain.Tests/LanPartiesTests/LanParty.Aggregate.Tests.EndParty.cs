using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void End_VotingClosed_EndsLanParty()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.EndParty(creator);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.IsEnded);
        Assert.IsTrue(lanParty.DomainEvents[0] is LanPartyEnded);
    }

    [TestMethod]
    public void End_NotModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var notModerator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.EndParty(notModerator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.NotAModerator);
    }

    [TestMethod]
    public void End_VotingOpen_ReturnsVotingNotClosed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.EndParty(creator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.VotingNotClosed);
    }

    [TestMethod]
    public void End_AlreadyEnded_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyEnded(creator);

        var result = lanParty.EndParty(creator);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.Ended);
    }
}
