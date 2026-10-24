using LanPlanner.Domain.LanParties;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

internal static class LanPartyTestData
{
    public static readonly DateOnly Start = new(2026, 11, 6);
    public static readonly DateOnly End = new(2026, 11, 8);

    public static UserId NewUser() => UserId.Create(Guid.NewGuid()).Value;

    internal static LanParty CreateMockedLanParty(UserId moderator)
    {
        var id = LanPartyId.Create(Guid.NewGuid()).Value;
        var creator = moderator;
        var name = "Mocked Lan Party";
        var start = Start;
        var end = End;
        var party = LanParty.Create(id, creator, name, start, end).Value;
        party.ClearDomainEvents();
        return party;
    }

    internal static LanParty CreateMockedLanPartyWithVotingClosed(UserId moderator)
    {
        var id = LanPartyId.Create(Guid.NewGuid()).Value;
        var creator = moderator;
        var name = "Mocked Lan Party";
        var start = Start;
        var end = End;
        var party = LanParty.Create(id, creator, name, start, end).Value;

        party.CloseVoting(moderator);
        party.ClearDomainEvents();
        return party;
    }
}