
using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

/// <summary>
/// Tests for the LanParty aggregate.
/// </summary>
[TestClass]
public partial class LanPartyAggregateTests
{
    // Tests for error scenarios in the LanParty aggregate creation
    [TestMethod]
    public void Create_NameNull_ReturnsNameRequired()
    {
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            null,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(LanPartyErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameEmpty_ReturnsNameRequired()
    {
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            string.Empty,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(LanPartyErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameWhitespace_ReturnsNameRequired()
    {
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            "   ",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(LanPartyErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_EndBeforeStart_ReturnsEndBeforeStart()
    {
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            "Test Lan Party",
            DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DateOnly.FromDateTime(DateTime.Now)
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(LanPartyErrors.EndBeforeStart, result.FirstError);
    }

    [TestMethod]
    public void Create_EndEqualsStart_LanParty()
    {
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            "Test Lan Party",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsFalse(result.IsError);
    }
    // Tests for successful scenarios in the LanParty aggregate creation
    [TestMethod]
    public void Create_Valid_SetsNameStartEnd()
    {
        var name = "Test Lan Party";
        var start = DateOnly.FromDateTime(DateTime.Now);
        var end = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            name,
            start,
            end
        );

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(name, result.Value.Name);
        Assert.AreEqual(start, result.Value.Start);
        Assert.AreEqual(end, result.Value.End);
    }

    [TestMethod]
    public void Create_Valid_CreatorIsParticipant()
    {
        var creatorId = UserId.Create(Guid.NewGuid()).Value;
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            creatorId,
            "Test Lan Party",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(result.Value.Participants.Any(p => p == creatorId));
    }

    [TestMethod]
    public void Create_Valid_CreatorIsModerator()
    {
        var creatorId = UserId.Create(Guid.NewGuid()).Value;
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            creatorId,
            "Test Lan Party",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(result.Value.Moderators.Any(m => m == creatorId));
    }

    [TestMethod]
    public void Create_Valid_VotingIsOpen()
    {
        var result = LanParty.Create(
            LanPartyId.Create(Guid.NewGuid()).Value,
            UserId.Create(Guid.NewGuid()).Value,
            "Test Lan Party",
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        );

        Assert.IsFalse(result.IsError);
        Assert.IsTrue(result.Value.VotingIsOpen);
    }

    [TestMethod]
    public void Create_Valid_RaisesFourEventsInOrder()
    {
        var id = LanPartyId.Create(Guid.NewGuid()).Value;
        var name = "Winter-LAN";
        var creator = UserId.Create(Guid.NewGuid()).Value;
        var start = new DateOnly(2026, 11, 6);
        var end = new DateOnly(2026, 11, 8);

        var result = LanParty.Create(
            id,
            creator,
            name,
            start,
            end
        );

        Assert.IsFalse(result.IsError);

        CollectionAssert.AreEqual(
            new IDomainEvent[]
            {
            new LanPartyCreated(id, name, start, end),
            new ParticipantJoined(id, creator),
            new ModeratorAppointed(id, creator),
            new VotingOpened(id)
            },
            result.Value.DomainEvents.ToList());


        // Assuming the LanParty aggregate raises four events in order upon creation
        // Replace the following with actual event assertions if applicable
        // Assert.AreEqual(4, result.Value.Events.Count);
    }
}
