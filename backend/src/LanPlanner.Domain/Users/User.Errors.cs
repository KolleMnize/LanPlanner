using ErrorOr;

namespace LanPlanner.Domain.Users;

public static class UserErrors
{
    // Errors related to user properties validation
    public static readonly Error EmailRequired = Error.Validation(
        code: "User.EmailRequired",
        description: "Email cannot be Null or empty.");

    public static readonly Error NameRequired = Error.Validation(
        code: "User.NameRequired",
        description: "Name cannot be Null or empty.");

    public static readonly Error UserIdCannotBeEmpty = Error.Validation(
        code: "User.UserIdCannotBeEmpty",
        description: "UserId cannot be empty.");

    // Errors which needed for application logic
    public static readonly Error ActorNotAdmin = Error.Forbidden(
        code: "User.ActorNotAdmin",
        description: "Actor must be an admin.");

    public static readonly Error EmailAlreadyRegistered = Error.Conflict(
        code: "User.EmailAlreadyRegistered",
        description: "Email is already registered.");
}
