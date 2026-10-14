using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Users;

public interface IUserRepository : IBaseRepository<User, UserId>
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The user with the specified email address wrapped in an ErrorOr result.
    /// Possible error types: <see cref="ErrorType.NotFound"/>, <see cref="ErrorType.Unexpected"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task<ErrorOr<User>> GetByEmail(string email, CancellationToken cancellationToken);
}

