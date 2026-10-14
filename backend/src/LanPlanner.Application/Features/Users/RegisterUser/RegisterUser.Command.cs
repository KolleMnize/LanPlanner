using LanPlanner.Application.Interfaces;

namespace LanPlanner.Application.Features.Users.RegisterUser;

public record RegisterUserCommand(string Email, string Name) : ICommand<RegisterUserCommandResult>;