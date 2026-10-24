namespace LanPlanner.Domain.SharedKernel;

public interface IAggregate
{
    public AggregateId Id { get; }
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }
    public void ClearDomainEvents();
}