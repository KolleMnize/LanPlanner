using LanPlanner.Domain.LanParties;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

[TestClass]
public partial class LanPartyIdValueObjectTests
{
    [TestMethod]
    public void Create_ValidGuid_ReturnsLanPartyId()
    {
        var guid = Guid.NewGuid();

        var result = LanPartyId.Create(guid);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(guid, result.Value.Value);
    }

    [TestMethod]
    public void Create_InvalidGuid_ReturnsError()
    {
        var guid = Guid.Empty;

        var result = LanPartyId.Create(guid);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(LanPartyErrors.LanPartyIdCannotBeEmpty, result.FirstError);
    }
}
