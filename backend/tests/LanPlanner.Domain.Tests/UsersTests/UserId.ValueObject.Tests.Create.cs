using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.Tests.UsersTests;

[TestClass]
public partial class UserIdValueObjectTests
{
    [TestMethod]
    public void Create_ValidGuid_ReturnsUserId()
    {
        var guid = Guid.NewGuid();

        var result = UserId.Create(guid);

        Assert.IsFalse(result.IsError);
        Assert.AreEqual(guid, result.Value.Value);
    }

    [TestMethod]
    public void Create_InvalidGuid_ReturnsError()
    {
        var guid = Guid.Empty;

        var result = UserId.Create(guid);

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(UserErrors.UserIdCannotBeEmpty, result.FirstError);
    }
}
