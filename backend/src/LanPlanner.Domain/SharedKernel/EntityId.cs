namespace LanPlanner.Domain.SharedKernel;

public abstract class EntityId : IEquatable<EntityId>
{
    public Guid Value { get; }

    protected EntityId(Guid value) => Value = value;

    public bool Equals(EntityId? other)
    {
        if (other is null) return false;
        return GetType() == other.GetType() && Value == other.Value;
    }

    public override bool Equals(object? obj) => Equals(obj as EntityId);
    public override int GetHashCode() => HashCode.Combine(GetType(), Value);
    public override string ToString() => Value.ToString();

    public static bool operator ==(EntityId? left, EntityId? right) =>
    left is null ? right is null : left.Equals(right);

    public static bool operator !=(EntityId? left, EntityId? right) =>
        !(left == right);
}