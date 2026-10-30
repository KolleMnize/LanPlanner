using LanPlanner.Domain.Users;
using LanPlanner.Domain.Users.Events;

namespace LanPlanner.Domain.Tests.UsersTests;

[TestClass]
public partial class UserAggregateTests
{
    [TestMethod]
    public void Create_NameNull_ReturnsNameRequired()
    {
        var result = User.Create(
            UserId.Create(Guid.NewGuid()).Value,
            "valid@email.com",
            null
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(UserErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_NameEmpty_ReturnsNameRequired()
    {
        var result = User.Create(
            UserId.Create(Guid.NewGuid()).Value,
            "valid@email.com",
            ""
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(UserErrors.NameRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_EmailNull_ReturnsEmailRequired()
    {
        var result = User.Create(
            UserId.Create(Guid.NewGuid()).Value,
            null,
            "Valid Name"
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(UserErrors.EmailRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_EmailEmpty_ReturnsEmailRequired()
    {
        var result = User.Create(
            UserId.Create(Guid.NewGuid()).Value,
            "",
            "Valid Name"
        );

        Assert.IsTrue(result.IsError);
        Assert.AreEqual(UserErrors.EmailRequired, result.FirstError);
    }

    [TestMethod]
    public void Create_Valid_ReturnsUserAndRaisesUserCreatedEvent()
    {
        var result = User.Create(
            UserId.Create(Guid.NewGuid()).Value,
            "valid@email.com",
            "Valid Name"
        );

        Assert.IsFalse(result.IsError);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("valid@email.com", result.Value.Email);
        Assert.AreEqual("Valid Name", result.Value.Name);
        Assert.IsFalse(result.Value.IsAdmin);
        Assert.IsTrue(result.Value.DomainEvents[0] is UserCreated userCreated && userCreated.Name == "Valid Name" && userCreated.UserId == result.Value.Id);
    }

}