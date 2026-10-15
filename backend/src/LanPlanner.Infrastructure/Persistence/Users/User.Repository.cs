using ErrorOr;
using LanPlanner.Domain.Users;
using LanPlanner.Infrastructure.Persistence.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LanPlanner.Infrastructure.Persistence.Users;

internal class UserRepository(ILogger<UserRepository> logger, LanPlannerDbContext dbContext) : BaseRepository<User, Guid>(logger, dbContext), IUserRepository
{
    public async Task<ErrorOr<User>> GetByEmail(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await DbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user is null)
                return Error.NotFound("User.NotFound", "User with the specified email was not found.");

            return user;
        }
        catch (OperationCanceledException)
        {
            logger.LogDebug("Operation was canceled while attempting to retrieve a User by email.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while attempting to retrieve a User by email.");
            return Error.Unexpected("Repository.UnexpectedError", "An error occurred while attempting to retrieve a User by email.");
        }
    }
}
