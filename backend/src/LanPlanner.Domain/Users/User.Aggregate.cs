using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Users;

public class User : Aggregate<UserId>
{
    public required string Email { get; init; }
    public required string Name { get; init; }

    private User() { } // Required by EF Core or other ORMs

    public static ErrorOr<User> Create(UserId id, string email, string name)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Error.Validation(
                code: "User.InvalidEmail",
                description: "Email cannot be empty.");
        }

        if (string.IsNullOrEmpty(name))
        {
            return Error.Validation(
                code: "User.InvalidName",
                description: "Name cannot be empty.");
        }

        return new User
        {
            Id = id,
            Email = email,
            Name = name
        };
    }
}