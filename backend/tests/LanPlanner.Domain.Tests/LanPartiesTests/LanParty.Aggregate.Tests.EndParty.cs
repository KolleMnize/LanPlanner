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
    }

    [TestMethod]
    public void End_VotingClosed_RaisesLanPartyEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        lanParty.EndParty(creator);

        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is LanPartyEnded));
    }

    [TestMethod]
    public void End_NotModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var notModerator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithVotingClosed(creator);

        var result = lanParty.EndParty(notModerator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotAModerator, result.Errors);
    }

    [TestMethod]
    public void End_VotingOpen_ReturnsVotingNotClosed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.EndParty(creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.VotingNotClosed, result.Errors);
    }

    [TestMethod]
    public void End_AlreadyEnded_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyEnded(creator);

        var result = lanParty.EndParty(creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.Ended, result.Errors);
    }

    [TestMethod]
    public void End_Invalid_KeepsStateAndRaisesNoEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyEnded(creator);

        var result = lanParty.EndParty(creator);

        Assert.IsTrue(result.IsError);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }
}
