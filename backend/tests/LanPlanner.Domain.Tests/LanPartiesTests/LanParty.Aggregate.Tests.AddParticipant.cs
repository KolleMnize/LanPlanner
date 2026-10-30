using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;


public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void Join_NewUser_AddsParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var newUser = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.Join(newUser);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(2, lanParty.Participants); // Assuming the creator is already a participant and the new user is added
        Assert.IsTrue(lanParty.Participants.Any(p => p == newUser));
        Assert.IsTrue(lanParty.DomainEvents[0] is ParticipantJoined pj && pj.UserId == newUser);
    }

    [TestMethod]
    public void Join_AlreadyParticipant_ReturnsAlreadyParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var newUser = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);
        lanParty.Join(newUser);
        lanParty.ClearDomainEvents();

        var result = lanParty.Join(newUser);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.AlreadyParticipant);
    }

    [TestMethod]
    public void Join_NewUser_IsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var newUser = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.Join(newUser);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.Moderators.Any(m => m == newUser));
        Assert.IsTrue(lanParty.DomainEvents[0] is ParticipantJoined pj && pj.UserId == newUser);
    }

    [TestMethod]
    public void Join_Ended_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var newUser = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyEnded(creator);

        var result = lanParty.Join(newUser);

        LanPartyAssert.FailedWith(result, lanParty, LanPartyErrors.Ended);
    }
}
