using ErrorOr;

namespace LanPlanner.Domain.Users;

public static class UserErrors
{
    public static readonly Error EmailRequired = Error.Validation(
        code: "User.EmailRequired",
        description: "Email cannot be Null or empty.");

    public static readonly Error NameRequired = Error.Validation(
        code: "User.NameRequired",
        description: "Name cannot be Null or empty.");

    public static readonly Error UserIdCannotBeEmpty = Error.Validation(
        code: "User.UserIdCannotBeEmpty",
        description: "UserId cannot be empty.");
}
