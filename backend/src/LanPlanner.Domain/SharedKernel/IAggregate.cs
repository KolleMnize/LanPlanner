namespace LanPlanner.Domain.SharedKernel;

public interface IAggregate
{
    public AggregateId Id { get; }
}