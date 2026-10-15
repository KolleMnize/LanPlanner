using ErrorOr;
using LanPlanner.Application.Interfaces;
using LanPlanner.Domain.Users;

namespace LanPlanner.Application.Features.Users.RegisterUser;

public class RegisterUserHandler(IUserRepository userRepository) : ICommandHandler<RegisterUserCommand, RegisterUserCommandResult>
{
    public async Task<ErrorOr<RegisterUserCommandResult>> Handle(RegisterUserCommand command, CancellationToken ct = default)
    {
        var userId = UserId.Create(Guid.NewGuid());
        if (userId.IsError)
            return userId.Errors;

        var userCreateResult = User.Create(userId.Value, command.Email, command.Name);
        if (userCreateResult.IsError)
            return userCreateResult.Errors;
        User user = userCreateResult.Value;

        var existing = await userRepository.GetByEmail(user.Email, ct);

        if (!existing.IsError && existing.Value is not null)
            return Error.Conflict(
                code: "User.AlreadyRegistered",
                description: "Der Nutzer ist bereits registriert.");

        if (existing.FirstError.Type != ErrorType.NotFound)
            return existing.Errors;

        var createdDbResult = await userRepository.Create(user, ct);
        if (createdDbResult.IsError)
            return createdDbResult.Errors;

        return new RegisterUserCommandResult(UserDtoMapper.UserToUserDto(user));

    }
}