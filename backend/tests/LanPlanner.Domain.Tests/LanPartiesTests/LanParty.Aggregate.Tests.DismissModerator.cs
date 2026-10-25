using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void DismissModerator_Valid_TargetIsNoLongerModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithTwoModerators(creator, participant);

        var result = lanParty.DismissModerator(creator, participant);

        Assert.IsFalse(result.IsError);
        Assert.HasCount(1, lanParty.Moderators);
        Assert.IsFalse(lanParty.Moderators.Any(p => p == participant));
    }

    [TestMethod]
    public void DismissModerator_Valid_TargetStaysParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithTwoModerators(creator, participant);

        var result = lanParty.DismissModerator(creator, participant);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.Participants.Any(p => p == participant));
    }

    [TestMethod]
    public void DismissModerator_Valid_RaisesModeratorDismissed()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithTwoModerators(creator, participant);

        var result = lanParty.DismissModerator(creator, participant);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is ModeratorDismissed mde && mde.UserId == participant));
    }

    [TestMethod]
    public void DismissModerator_Self_WithOtherModerator_Succeeds()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var secondModerator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithTwoModerators(creator, secondModerator);

        var result = lanParty.DismissModerator(creator, creator);

        Assert.IsFalse(result.IsError);
        Assert.IsFalse(lanParty.Moderators.Any(p => p == creator));
        Assert.IsTrue(lanParty.Moderators.Any(p => p == secondModerator));
    }

    [TestMethod]
    public void DismissModerator_Self_OnlyModerator_ReturnsLastModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.DismissModerator(creator, creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.LastModerator, result.Errors);
        Assert.IsTrue(lanParty.Moderators.Any(p => p == creator));
    }

    [TestMethod]
    public void DismissModerator_ActorParticipantNotModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.DismissModerator(participant, creator);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotAModerator, result.Errors);
        Assert.IsTrue(lanParty.Moderators.Any(p => p == creator));
    }

    [TestMethod]
    public void DismissModerator_Ended_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithTwoModerators(creator, participant);
        lanParty.CloseVoting(creator);
        lanParty.EndParty(creator);

        var result = lanParty.DismissModerator(creator, participant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.Ended, result.Errors);
        Assert.IsTrue(lanParty.Moderators.Any(p => p == participant));
    }

    [TestMethod]
    public void DismissModerator_TargetParticipantNotModerator_ReturnsTargetNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var result = lanParty.DismissModerator(creator, participant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.TargetNotModerator, result.Errors);
        Assert.IsTrue(lanParty.Moderators.Any(p => p == creator));
    }

    [TestMethod]
    public void DismissModerator_TargetNotParticipant_ReturnsTargetNotParticipant()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var nonParticipant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.DismissModerator(creator, nonParticipant);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.TargetNotParticipant, result.Errors);
        Assert.IsTrue(lanParty.Moderators.Any(p => p == creator));
    }

    [TestMethod]
    public void DismissModerator_Invalid_KeepsStateAndRaisesNoEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var result = lanParty.DismissModerator(creator, UserId.Create(Guid.NewGuid()).Value);

        Assert.IsTrue(result.IsError);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }

}