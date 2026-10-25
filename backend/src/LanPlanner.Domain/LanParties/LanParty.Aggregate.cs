using ErrorOr;
using LanPlanner.Domain.Games;
using LanPlanner.Domain.LanParties.Events;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties;

public class LanParty : Aggregate<LanPartyId>
{
    private List<Participant> participants = [];
    private List<Proposal> proposals = [];
    public string Name { get; private set; } = null!;
    public DateOnly Start { get; private set; }
    public DateOnly End { get; private set; }
    public bool VotingIsOpen { get; private set; }
    public bool IsEnded { get; private set; }

    public IReadOnlyList<UserId> Participants => participants.Select(p => p.UserId).ToList().AsReadOnly();
    public IReadOnlyList<UserId> Moderators => participants.Where(p => p.IsModerator).Select(p => p.UserId).ToList().AsReadOnly();
    public IReadOnlyList<GameModeId> Proposals => proposals.Select(p => p.GameModeId).ToList().AsReadOnly();

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

    public ErrorOr<Success> DismissModerator(UserId actor, UserId userId)
    {
        if (!IsModerator(actor))
            return LanPartyErrors.NotAModerator;

        if (IsEnded)
            return LanPartyErrors.Ended;

        var participant = FindParticipant(userId);
        if (participant is null)
            return LanPartyErrors.TargetNotParticipant;

        if (!participant.IsModerator)
            return LanPartyErrors.TargetNotModerator;

        if (participants.Count(p => p.IsModerator) == 1)
            return LanPartyErrors.LastModerator;

        RemoveModerator(participant);
        return Result.Success;
    }

    public ErrorOr<Success> Propose(UserId actor, GameModeId gameModeId)
    {
        if (!IsParticipant(actor))
            return LanPartyErrors.NotAParticipant;

        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        if (FindProposal(gameModeId) is not null)
            return LanPartyErrors.GameModeAlreadyProposed;

        AddProposal(actor, gameModeId);
        return Result.Success;
    }

    public ErrorOr<Success> CastVote(UserId actor, GameModeId gameModeId, VoteDirection direction)
    {
        if (!IsParticipant(actor))
            return LanPartyErrors.NotAParticipant;

        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        var proposal = FindProposal(gameModeId);
        if (proposal is null)
            return LanPartyErrors.GameModeNotProposed;

        var existingVote = proposal.FindVote(actor);
        if (existingVote is not null)
            return LanPartyErrors.AlreadyVoted;

        AddVote(proposal, actor, direction);
        return Result.Success;
    }

    public VoteDirection? GetVote(UserId userId, GameModeId gameModeId)
    {
        return FindProposal(gameModeId)?.FindVote(userId)?.Direction;
    }

    public ErrorOr<Success> ChangeVote(UserId actor, GameModeId gameModeId)
    {
        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        var proposal = FindProposal(gameModeId);
        if (proposal is null)
            return LanPartyErrors.GameModeNotProposed;

        var existingVote = proposal.FindVote(actor);
        if (existingVote is null)
            return LanPartyErrors.NotVoted;

        FlipVote(proposal, existingVote);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveVote(UserId actor, GameModeId gameModeId)
    {
        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        var proposal = FindProposal(gameModeId);
        if (proposal is null)
            return LanPartyErrors.GameModeNotProposed;

        var existingVote = proposal.FindVote(actor);
        if (existingVote is null)
            return LanPartyErrors.NotVoted;

        RemoveVoteInternal(proposal, existingVote);
        return Result.Success;
    }

    public GameModeId? GetVeto(UserId userId)
    {
        return FindProposalVetoedBy(userId)?.GameModeId;
    }

    public ErrorOr<Success> CastVeto(UserId actor, GameModeId gameModeId)
    {
        if (!IsParticipant(actor))
            return LanPartyErrors.NotAParticipant;

        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        var proposal = FindProposal(gameModeId);
        if (proposal is null)
            return LanPartyErrors.GameModeNotProposed;

        var existingVeto = FindProposalVetoedBy(actor);
        if (existingVeto is not null)
            return LanPartyErrors.AlreadyVetoed;

        AddVeto(proposal, actor);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveVeto(UserId actor)
    {
        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        var proposal = FindProposalVetoedBy(actor);
        if (proposal is null)
            return LanPartyErrors.NotVetoed;

        RemoveVetoInternal(proposal, actor);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveProposal(UserId removedBy, GameModeId gameModeId)
    {
        if (!IsModerator(removedBy))
            return LanPartyErrors.NotAModerator;

        if (!VotingIsOpen)
            return LanPartyErrors.VotingNotOpen;

        var proposal = FindProposal(gameModeId);
        if (proposal is null)
            return LanPartyErrors.GameModeNotProposed;

        RemoveProposalInternal(proposal, removedBy);
        return Result.Success;
    }

    public ErrorOr<Success> Leave(UserId userId)
    {
        Participant? participant = FindParticipant(userId);

        if (participant is null)
            return LanPartyErrors.NotAParticipant;

        if (IsEnded)
            return LanPartyErrors.Ended;

        if (participant.IsModerator && Moderators.Count == 1)
            return LanPartyErrors.LastModerator;

        RemoveParticipantInternal(participant);
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
    private bool IsParticipant(UserId userId)
        => participants.Any(p => p.UserId == userId);

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

    private void RemoveModerator(Participant participant)
    {
        participant.Demote();
        Raise(new ModeratorDismissed(Id, participant.UserId));
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

    private void AddProposal(UserId actor, GameModeId gameModeId)
    {
        var proposal = Proposal.Create(gameModeId);
        proposals.Add(proposal);
        Raise(new GameModeProposed(Id, actor, gameModeId));
    }

    private Proposal? FindProposal(GameModeId gameModeId)
        => proposals.Find(p => p.GameModeId == gameModeId);

    private void AddVote(Proposal proposal, UserId userId, VoteDirection direction)
    {
        proposal.AddVote(Vote.Create(userId, direction));

        IDomainEvent domainEvent = direction switch
        {
            VoteDirection.Up => new GameModeUpvoted(Id, userId, proposal.GameModeId),
            VoteDirection.Down => new GameModeDownvoted(Id, userId, proposal.GameModeId),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
        Raise(domainEvent);
    }

    private void FlipVote(Proposal proposal, Vote vote)
    {
        vote.Flip();
        Raise(new VoteChanged(Id, vote.UserId, proposal.GameModeId, vote.Direction));
    }

    private void RemoveVoteInternal(Proposal proposal, Vote vote)
    {
        proposal.RemoveVote(vote);
        Raise(new VoteRemoved(Id, vote.UserId, proposal.GameModeId, vote.Direction));
    }

    private Proposal? FindProposalVetoedBy(UserId userId)
        => proposals.Find(p => p.HasVetoFrom(userId));

    private void AddVeto(Proposal proposal, UserId userId)
    {
        proposal.AddVeto(userId);
        Raise(new GameModeVetoed(Id, userId, proposal.GameModeId));
    }
    private void RemoveVetoInternal(Proposal proposal, UserId userId)
    {
        proposal.RemoveVeto(userId);
        Raise(new VetoRemoved(Id, userId, proposal.GameModeId));
    }
    private void RemoveProposalInternal(Proposal proposal, UserId removedBy)
    {
        proposals.Remove(proposal);

        Raise(new ProposalRemoved(Id, removedBy, proposal.GameModeId));

        foreach (var vote in proposal.Votes)
            Raise(new VoteRemoved(Id, vote.UserId, proposal.GameModeId, vote.Direction));

        foreach (var veto in proposal.Vetos)
            Raise(new VetoRemoved(Id, veto, proposal.GameModeId));
    }

    private void RemoveParticipantInternal(Participant participant)
    {
        var userId = participant.UserId;

        participants.Remove(participant);
        Raise(new ParticipantLeft(Id, userId));

        foreach (var proposal in proposals)
        {
            if (proposal.FindVote(userId) is { } vote)
                RemoveVoteInternal(proposal, vote);
        }

        if (FindProposalVetoedBy(userId) is { } veto)
            RemoveVetoInternal(veto, userId);
    }
}