using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void AppointModerator_Valid_TargetIsModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.AppointModerator(creator, participant);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(2, lanParty.Moderators); // Assuming the creator is already a participant and the new participant is added
        Assert.IsTrue(lanParty.Moderators.Any(p => p == participant));
    }

    [TestMethod]
    public void AppointModerator_Valid_TargetStaysParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.AppointModerator(creator, participant);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(2, lanParty.Participants); // Assuming the creator is already a participant and the new participant is added
        Assert.IsTrue(lanParty.Participants.Any(p => p == participant));
    }

    [TestMethod]
    public void AppointModerator_Valid_RaisesModeratorAppointed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.AppointModerator(creator, participant);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is ModeratorAppointed mae && mae.UserId == participant));
    }

    [TestMethod]
    public void AppointModerator_ActorParticipantNotModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.AppointModerator(participant, participant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotAModerator, result.Errors);
    }

    [TestMethod]
    public void AppointModerator_Ended_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.CloseVoting(creator);
        lanParty.EndParty(creator);

        var result = lanParty.AppointModerator(creator, participant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.Ended, result.Errors);
    }

    [TestMethod]
    public void AppointModerator_TargetNotParticipant_ReturnsTargetNotParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var nonParticipant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, UserId.Create(Guid.NewGuid()).Value);

        var result = lanParty.AppointModerator(creator, nonParticipant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.TargetNotParticipant, result.Errors);
    }

    [TestMethod]
    public void AppointModerator_TargetAlreadyModerator_ReturnsTargetAlreadyModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);
        lanParty.AppointModerator(creator, participant);

        var result = lanParty.AppointModerator(creator, participant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.TargetAlreadyModerator, result.Errors);
    }

    [TestMethod]
    public void AppointModerator_Invalid_KeepsStateAndRaisesNoEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.AppointModerator(creator, UserId.Create(Guid.NewGuid()).Value);

        Assert.IsTrue(result.IsError);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }
}
