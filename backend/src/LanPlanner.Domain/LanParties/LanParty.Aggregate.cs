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
    public bool IsEnded { get; private set; }

    public IReadOnlyList<UserId> Participants => participants.AsReadOnly();
    public IReadOnlyList<UserId> Moderators => moderators.AsReadOnly();

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

        party.AddParticipant(creator);
        party.AddModerator(creator);
        party.OpenVotingInternal();

        return party;
    }

    public ErrorOr<Success> CloseVoting(UserId actor)
    {
        if (!moderators.Contains(actor))
            return LanPartyErrors.NotAModerator;

        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        CloseVotingInternal();
        return Result.Success;
    }

    public ErrorOr<Success> EndParty(UserId actor)
    {
        if (!moderators.Contains(actor))
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
        if (!moderators.Contains(actor))
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

        if (participants.Contains(actor))
            return LanPartyErrors.AlreadyParticipant;

        AddParticipant(actor);
        return Result.Success;
    }

    public ErrorOr<Success> ChangeDetails(UserId actor, string name, DateOnly start, DateOnly end)
    {
        if (!moderators.Contains(actor))
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