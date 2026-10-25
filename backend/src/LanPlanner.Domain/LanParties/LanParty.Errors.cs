using ErrorOr;

namespace LanPlanner.Domain.LanParties;

public static class LanPartyErrors
{
    // Identity errors
    public static readonly Error LanPartyIdCannotBeEmpty = Error.Validation(
        code: "LanPartyId.CannotBeEmpty",
        description: "LanPartyId cannot be empty.");

    // Details and lifecycle errors
    public static readonly Error NameRequired = Error.Validation(
        code: "LanParty.NameRequired",
        description: "Name must not be empty.");

    public static readonly Error EndBeforeStart = Error.Validation(
        code: "LanParty.EndBeforeStart",
        description: "End date must not be before start date.");

    public static readonly Error Ended = Error.Conflict(
        code: "LanParty.Ended",
        description: "The LAN party has already ended.");

    // Participant and moderator errors
    public static readonly Error AlreadyParticipant = Error.Conflict(
        code: "LanParty.AlreadyParticipant",
        description: "User is already a participant.");

    public static readonly Error TargetNotParticipant = Error.Validation(
        code: "LanParty.TargetNotParticipant",
        description: "The target user is not a participant.");

    public static readonly Error NotAParticipant = Error.Forbidden(
        code: "LanParty.NotAParticipant",
        description: "User is not a participant.");

    public static readonly Error NotAModerator = Error.Forbidden(
        code: "LanParty.NotAModerator",
        description: "User is not a moderator.");

    public static readonly Error TargetAlreadyModerator = Error.Conflict(
        code: "LanParty.TargetAlreadyModerator",
        description: "The target user is already a moderator.");

    public static readonly Error TargetNotModerator = Error.Validation(
        code: "LanParty.TargetNotModerator",
        description: "The target user is not a moderator.");

    public static readonly Error LastModerator = Error.Conflict(
        code: "LanParty.LastModerator",
        description: "Cannot remove the last moderator.");

    // Voting errors
    public static readonly Error VotingNotOpen = Error.Conflict(
        code: "LanParty.VotingNotOpen",
        description: "Voting is not open.");

    public static readonly Error VotingNotClosed = Error.Conflict(
        code: "LanParty.VotingNotClosed",
        description: "Voting is not closed.");

    public static readonly Error GameModeNotProposed = Error.NotFound(
        code: "LanParty.GameModeNotProposed",
        description: "The game mode has not been proposed.");

    public static readonly Error AlreadyVoted = Error.Conflict(
        code: "LanParty.AlreadyVoted",
        description: "User has already voted.");

    public static readonly Error NotVoted = Error.NotFound(
        code: "LanParty.NotVoted",
        description: "User has not voted."
    );

    // Proposal errors
    public static readonly Error GameModeAlreadyProposed = Error.Conflict(
        code: "LanParty.GameModeAlreadyProposed",
        description: "The game mode has already been proposed.");

    // Veto Errors
    public static readonly Error AlreadyVetoed = Error.Conflict(
        code: "LanParty.UserAlreadyVetoed",
        description: "The User has already vetoed.");

    public static readonly Error NotVetoed = Error.NotFound(
        code: "LanParty.NotVetoed",
        description: "The user has not vetoed this game mode.");
}