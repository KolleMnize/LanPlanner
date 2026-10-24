using ErrorOr;

namespace LanPlanner.Domain.LanParties;

public static class LanPartyErrors
{
    /// Id related errors
    public static readonly Error LanPartyIdCannotBeEmpty = Error.Validation(
        code: "LanPartyId.CannotBeEmpty",
        description: "LanPartyId cannot be empty.");

    // create related errors
    public static readonly Error NameRequired = Error.Validation(
        code: "LanParty.NameRequired",
        description: "Name must not be empty.");

    public static readonly Error EndBeforeStart = Error.Validation(
        code: "LanParty.EndBeforeStart",
        description: "End date must not be before start date.");

    // voting related errors
    public static readonly Error NotAModerator = Error.Forbidden(
        code: "LanParty.NotAModerator",
        description: "User is not a moderator.");

    public static readonly Error VotingNotOpen = Error.Conflict(
        code: "LanParty.VotingNotOpen",
        description: "Voting is not open.");

}