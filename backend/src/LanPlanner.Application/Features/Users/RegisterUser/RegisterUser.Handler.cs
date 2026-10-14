using ErrorOr;
using LanPlanner.Application.Interfaces;
using LanPlanner.Domain.Users;

namespace LanPlanner.Application.Features.Users.RegisterUser;

public class RegisterUserHandler(IUserRepository userRepository) : ICommandHandler<RegisterUserCommand, RegisterUserCommandResult>
{
    public async Task<ErrorOr<RegisterUserCommandResult>> Handle(RegisterUserCommand command, CancellationToken ct = default)
    {
        var userCreateResult = User.Create(UserId.Create(new Guid()).Value, command.Email, command.Name);
        User user = userCreateResult.Value;

        if (userCreateResult.IsError)
            return userCreateResult.Errors;

        var existing = await userRepository.GetByEmail(user.Email, ct);

        if (existing.IsError)
            return existing.Errors;

        if (existing.Value is not null)
            return Error.Conflict(
                code: "User.AlreadyRegistered",
                description: "Der Nutzer ist bereits registriert.");

        var createdDbResult = await userRepository.Create(user, ct);
        if (createdDbResult.IsError)
            return createdDbResult.Errors;

        return new RegisterUserCommandResult(UserDtoMapper.UserToUserDto(user));

    }
}