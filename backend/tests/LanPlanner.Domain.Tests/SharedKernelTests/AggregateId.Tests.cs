using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Tests.SharedKernelTests;

[TestClass]
public class AggregateIdTests
{

    private sealed class TestId(Guid value) : AggregateId(value);
    private sealed class TestAggregate : Aggregate<TestId> { }

    // --- Equals ---

    [TestMethod]
    public void Equals_Null_ReturnsFalse()
    {
        var id = new TestId(Guid.NewGuid());

        Assert.IsFalse(id.Equals((AggregateId?)null));
    }

    [TestMethod]
    public void Equals_ObjectOfOtherType_ReturnsFalse()
    {
        var guid = Guid.NewGuid();

        Assert.IsFalse(new TestId(guid).Equals((object)guid));
    }

    [TestMethod]
    public void Equals_AsObjectSameValue_ReturnsTrue()
    {
        var guid = Guid.NewGuid();

        Assert.IsTrue(new TestId(guid).Equals((object)new TestId(guid)));
    }

    // --- Operatoren: fehlende Zweige ---

    [TestMethod]
    public void EqualsOperator_LeftNullRightNotNull_ReturnsFalse()
    {
        TestId? left = null;

        Assert.IsFalse(left == new TestId(Guid.NewGuid()));
    }

    [TestMethod]
    public void NotEqualsOperator_SameValue_ReturnsFalse()
    {
        var guid = Guid.NewGuid();

        Assert.IsFalse(new TestId(guid) != new TestId(guid));
    }

    // --- GetHashCode ---

    [TestMethod]
    public void GetHashCode_SameValue_SameHash()
    {
        var guid = Guid.NewGuid();

        Assert.AreEqual(new TestId(guid).GetHashCode(), new TestId(guid).GetHashCode());
    }

    [TestMethod]
    public void HashSet_SameValueTwice_ContainsOne()
    {
        var guid = Guid.NewGuid();
        var set = new HashSet<AggregateId> { new TestId(guid), new TestId(guid) };

        Assert.HasCount(1, set);
    }

    // --- ToString ---

    [TestMethod]
    public void ToString_ReturnsGuid()
    {
        var guid = Guid.NewGuid();

        Assert.AreEqual(guid.ToString(), new TestId(guid).ToString());
    }


}
