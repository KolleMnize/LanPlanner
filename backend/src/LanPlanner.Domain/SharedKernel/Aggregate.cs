
namespace LanPlanner.Domain.SharedKernel;

public abstract class Aggregate<TId> : IAggregate
    where TId : AggregateId
{
    public required TId Id { get; init; }

    AggregateId IAggregate.Id => Id;

    private readonly List<IDomainEvent> domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => domainEvents.Clear();
}