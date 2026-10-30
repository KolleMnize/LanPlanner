using ErrorOr;
using LanPlanner.Domain.SharedKernel;
using LanPlanner.Domain.Users.Events;

namespace LanPlanner.Domain.Users;

public class User : Aggregate<UserId>
{
    public required string Email { get; init; }
    public required string Name { get; init; }
    public bool IsAdmin { get; init; } = false;

    private User() { } // Required by EF Core or other ORMs

    public static ErrorOr<User> Create(UserId id, string email, string name)
    {
        if (string.IsNullOrEmpty(email))
        {
            return UserErrors.EmailRequired;
        }

        if (string.IsNullOrEmpty(name))
        {
            return UserErrors.NameRequired;
        }

        var newUser = new User
        {
            Id = id,
            Email = email,
            Name = name,
        };

        newUser.Raise(new UserCreated(newUser.Id, newUser.Name));

        return newUser;
    }
}