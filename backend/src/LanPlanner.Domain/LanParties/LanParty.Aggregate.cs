using ErrorOr;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class LanParty : Aggregate<LanPartyId>
{
    private List<UserId> participants = [];
    private List<UserId> moderators = [];

    public string Name { get; private set; } = null!;
    public DateOnly Start { get; private set; }
    public DateOnly End { get; private set; }
    public bool VotingIsOpen { get; private set; }

    public IReadOnlyList<UserId> Participants => participants.AsReadOnly();
    public IReadOnlyList<UserId> Moderators => moderators.AsReadOnly();

    private LanParty() { } // Required by EF Core or other ORMs

    public static ErrorOr<LanParty> Create(LanPartyId id, UserId creator, string name, DateOnly start, DateOnly end)
    {
        if (string.IsNullOrWhiteSpace(name))
            return LanPartyErrors.NameRequired;

        if (end < start)
            return LanPartyErrors.EndBeforeStart;

        var party = new LanParty
        {
            Id = id,
            Name = name,
            Start = start,
            End = end
        };
        party.Raise(new LanPartyCreated(party.Id, party.Name, party.Start, party.End));

        party.AddParticipant(creator);
        party.AddModerator(creator);
        party.OpenVoting();

        return party;
    }

    private void AddParticipant(UserId userId)
    {
        participants.Add(userId);
        Raise(new ParticipantJoined(Id, userId));
    }

    private void AddModerator(UserId userId)
    {
        moderators.Add(userId);
        Raise(new ModeratorAppointed(Id, userId));
    }

    private void OpenVoting()
    {
        VotingIsOpen = true;
        Raise(new VotingOpened(Id));
    }

    private void CloseVoting()
    {
        VotingIsOpen = false;
        Raise(new VotingClosed(Id));
    }

}