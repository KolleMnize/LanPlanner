using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public partial class LanPartyAggregateTests
{
    [TestMethod]
    public void ChangeDetails_Valid_UpdatesNameStartEnd()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var newName = "New Name";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(21));
        var result = lanParty.ChangeDetails(creator, newName, newStart, newEnd);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual("New Name", lanParty.Name);
        Assert.AreEqual(newStart, lanParty.Start);
        Assert.AreEqual(newEnd, lanParty.End);
    }

    [TestMethod]
    public void ChangeDetails_Valid_RaisesLanPartyDetailsChanged()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var newName = "New Name";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(21));
        var result = lanParty.ChangeDetails(creator, newName, newStart, newEnd);

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(lanParty.DomainEvents.Any(e => e is LanPartyDetailsChanged));
    }

    [TestMethod]
    public void ChangeDetails_ParticipantNotModerator_ReturnsNotModerator()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var participant = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyWithParticipant(creator, participant);

        var newName = "New Name";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(21));
        var result = lanParty.ChangeDetails(participant, newName, newStart, newEnd);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NotAModerator, result.Errors);
    }

    [TestMethod]
    public void ChangeDetails_Ended_ReturnsEnded()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanPartyEnded(creator);

        var newName = "New Name";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(21));
        var result = lanParty.ChangeDetails(creator, newName, newStart, newEnd);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.Ended, result.Errors);
    }

    [TestMethod]
    public void ChangeDetails_NameEmpty_ReturnsNameRequired()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var newName = "";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(21));
        var result = lanParty.ChangeDetails(creator, newName, newStart, newEnd);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NameRequired, result.Errors);
    }

    [TestMethod]
    public void ChangeDetails_EndBeforeStart_ReturnsEndBeforeStart()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var newName = "New Name";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(19));
        var result = lanParty.ChangeDetails(creator, newName, newStart, newEnd);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.EndBeforeStart, result.Errors);
    }

    [TestMethod]
    public void ChangeDetails_Invalid_KeepsOldDataAndRaisesNoEvent()
    {
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var lanParty = LanPartyTestData.CreateMockedLanParty(creator);

        var oldName = lanParty.Name;
        var oldStart = lanParty.Start;
        var oldEnd = lanParty.End;

        var newName = "";
        var newStart = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
        var newEnd = DateOnly.FromDateTime(DateTime.Now.AddDays(19));
        var result = lanParty.ChangeDetails(creator, newName, newStart, newEnd);

        Assert.IsTrue(result.IsError);
        Assert.Contains(LanPartyErrors.NameRequired, result.Errors);
        Assert.AreEqual(oldName, lanParty.Name);
        Assert.AreEqual(oldStart, lanParty.Start);
        Assert.AreEqual(oldEnd, lanParty.End);
        Assert.IsFalse(lanParty.DomainEvents.Any());
    }
}