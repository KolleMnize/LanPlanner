using ErrorOr;
using LanPlanner.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LanPlanner.Infrastructure.Persistence.Base;

internal abstract class BaseRepository<TEntity, TId>(ILogger logger, LanPlannerDbContext dbContext)
    : IBaseRepository<TEntity, TId>
    where TEntity : class
{
    internal LanPlannerDbContext DbContext { get; } = dbContext;

    public async Task<ErrorOr<Success>> Create(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
        catch (OperationCanceledException)
        {
            logger.LogDebug("Operation was canceled while attempting to create an Aggregate.");
            throw;
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Create blocked by a foreign key constraint (likely a race condition).");
            return Error.Conflict("Repository.CreateConflict",
                "The entity could not be created because it is still referenced by another Aggregate.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while attempting to create an Aggregate.");
            return Error.Unexpected("Repository.UnexpectedError", "An error occurred while attempting to create an Aggregate.");
        }
    }
}