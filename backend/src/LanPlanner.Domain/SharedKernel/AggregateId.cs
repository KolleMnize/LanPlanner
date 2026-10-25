namespace LanPlanner.Domain.SharedKernel;

public abstract class AggregateId : EntityId
{
    protected AggregateId(Guid value) : base(value) { }
}
