using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Tests.SharedKernel;

[TestClass]
public class AggregateTests
{
    // Testhilfen – nur für diesen Test
    private sealed class TestId(Guid value) : AggregateId(value);
    private sealed record TestEvent(string Text) : IDomainEvent;
    private sealed class TestAggregate : Aggregate<TestId>
    {
        public void TuEtwas(string text) => Raise(new TestEvent(text));
    }

    private static TestAggregate NeuesAggregate() =>
        new() { Id = new TestId(Guid.NewGuid()) };

    [TestMethod]
    public void Neues_Aggregate_hat_keine_Events()
    {
        var aggregate = NeuesAggregate();

        Assert.IsEmpty(aggregate.DomainEvents);
    }

    [TestMethod]
    public void RaiseFuegtEventHinzu()
    {
        var aggregate = NeuesAggregate();

        aggregate.TuEtwas("a");

        Assert.HasCount(1, aggregate.DomainEvents);
        Assert.AreEqual(new TestEvent("a"), aggregate.DomainEvents[0]);
    }

    [TestMethod]
    public void Mehrere_Events_bleiben_in_Reihenfolge()
    {
        var aggregate = NeuesAggregate();

        aggregate.TuEtwas("a");
        aggregate.TuEtwas("b");

        Assert.AreEqual(new TestEvent("a"), aggregate.DomainEvents[0]);
        Assert.AreEqual(new TestEvent("b"), aggregate.DomainEvents[1]);
    }

    [TestMethod]
    public void ClearDomainEvents_leert_die_Liste()
    {
        var aggregate = NeuesAggregate();
        aggregate.TuEtwas("a");

        aggregate.ClearDomainEvents();

        Assert.IsEmpty(aggregate.DomainEvents);
    }
}