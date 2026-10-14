using LanPlanner.Domain.Users;

namespace LanPlanner.Application.Features.Users;

internal static class UserDtoMapper
{
    public static UserDto UserToUserDto(User user)
        => new UserDto(user.Email, user.Name);

}
