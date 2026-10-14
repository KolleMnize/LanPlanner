using ErrorOr;

namespace LanPlanner.Domain.SharedKernel;

/// <summary>
/// Represents a generic repository interface for CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public interface IBaseRepository<TEntity, TId>
{
    /// <summary>
    /// Creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A success result wrapped in an ErrorOr result.
    /// Possible error types: <see cref="ErrorType.Unexpected"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public Task<ErrorOr<Success>> Create(TEntity entity, CancellationToken cancellationToken);
}