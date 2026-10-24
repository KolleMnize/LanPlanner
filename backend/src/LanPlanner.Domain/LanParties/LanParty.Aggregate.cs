using ErrorOr;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class LanParty : Aggregate<LanPartyId>
{
    private List<Participant> participants = [];
    public string Name { get; private set; } = null!;
    public DateOnly Start { get; private set; }
    public DateOnly End { get; private set; }
    public bool VotingIsOpen { get; private set; }
    public bool IsEnded { get; private set; }

    public IReadOnlyList<UserId> Participants => participants.Select(p => p.UserId).ToList().AsReadOnly();
    public IReadOnlyList<UserId> Moderators => participants.Where(p => p.IsModerator).Select(p => p.UserId).ToList().AsReadOnly();

    private LanParty() { } // Required by EF Core or other ORMs

    public static ErrorOr<LanParty> Create(LanPartyId id, UserId creator, string name, DateOnly start, DateOnly end)
    {
        var validationResult = ValidateDetails(name, start, end);
        if (validationResult.IsError)
            return validationResult.Errors;

        var party = new LanParty
        {
            Id = id,
            Name = name,
            Start = start,
            End = end
        };
        party.Raise(new LanPartyCreated(party.Id, party.Name, party.Start, party.End));

        var creatorParticipant = party.AddParticipant(creator);
        party.AddModerator(creatorParticipant);
        party.OpenVotingInternal();

        return party;
    }

    public ErrorOr<Success> CloseVoting(UserId actor)
    {
        if (!IsModerator(actor))
            return LanPartyErrors.NotAModerator;

        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        CloseVotingInternal();
        return Result.Success;
    }

    public ErrorOr<Success> EndParty(UserId actor)
    {
        if (!IsModerator(actor))
            return LanPartyErrors.NotAModerator;

        if (IsEnded)
            return LanPartyErrors.Ended;

        if (VotingIsOpen)
            return LanPartyErrors.VotingNotClosed;

        EndPartyInternal();
        return Result.Success;
    }

    public ErrorOr<Success> OpenVoting(UserId actor)
    {
        if (!IsModerator(actor))
            return LanPartyErrors.NotAModerator;

        if (IsEnded)
            return LanPartyErrors.Ended;

        if (VotingIsOpen)
            return LanPartyErrors.VotingNotClosed;

        OpenVotingInternal();
        return Result.Success;
    }

    public ErrorOr<Success> Join(UserId actor)
    {
        if (IsEnded)
            return LanPartyErrors.Ended;

        if (participants.Any(p => p.UserId == actor))
            return LanPartyErrors.AlreadyParticipant;

        AddParticipant(actor);
        return Result.Success;
    }

    public ErrorOr<Success> ChangeDetails(UserId actor, string name, DateOnly start, DateOnly end)
    {
        if (!IsModerator(actor))
            return LanPartyErrors.NotAModerator;

        if (IsEnded)
            return LanPartyErrors.Ended;

        var validationResult = ValidateDetails(name, start, end);
        if (validationResult.IsError)
            return validationResult.Errors;

        Name = name;
        Start = start;
        End = end;

        Raise(new LanPartyDetailsChanged(Id, Name, Start, End));
        return Result.Success;
    }

    public ErrorOr<Success> AppointModerator(UserId actor, UserId userId)
    {
        if (!IsModerator(actor))
            return LanPartyErrors.NotAModerator;

        if (IsEnded)
            return LanPartyErrors.Ended;

        var participant = FindParticipant(userId);
        if (participant is null)
            return LanPartyErrors.TargetNotParticipant;

        if (participant.IsModerator)
            return LanPartyErrors.TargetAlreadyModerator;

        AddModerator(participant);
        return Result.Success;
    }

    private static ErrorOr<Success> ValidateDetails(string name, DateOnly start, DateOnly end)
    {
        List<Error> errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(LanPartyErrors.NameRequired);

        if (end < start)
            errors.Add(LanPartyErrors.EndBeforeStart);

        if (errors.Count > 0)
            return errors;

        return Result.Success;
    }

    private Participant? FindParticipant(UserId userId)
        => participants.Find(p => p.UserId == userId);

    private bool IsModerator(UserId userId)
        => participants.Any(p => p.UserId == userId && p.IsModerator);

    private Participant AddParticipant(UserId userId)
    {
        var participant = Participant.Create(userId);
        participants.Add(participant);
        Raise(new ParticipantJoined(Id, userId));
        return participant;
    }

    private void AddModerator(Participant participant)
    {
        participant.Promote();
        Raise(new ModeratorAppointed(Id, participant.UserId));
    }

    private void OpenVotingInternal()
    {
        VotingIsOpen = true;
        Raise(new VotingOpened(Id));
    }

    private void CloseVotingInternal()
    {
        VotingIsOpen = false;
        Raise(new VotingClosed(Id));
    }

    private void EndPartyInternal()
    {
        IsEnded = true;
        Raise(new LanPartyEnded(Id));
    }

}