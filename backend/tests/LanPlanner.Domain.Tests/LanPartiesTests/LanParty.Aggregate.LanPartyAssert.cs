using ErrorOr;
using LanPlanner.Domain.LanParties;

namespace LanPlanner.Domain.Tests.LanPartiesTests;

public static class LanPartyAssert
{
    public static void FailedWith(ErrorOr<Success> result, LanParty party, Error expected)
    {
        Assert.IsTrue(result.IsError, "Expected an error, but the command succeeded.");
        Assert.AreEqual(expected.Code, result.FirstError.Code);
        Assert.IsEmpty(party.DomainEvents, "No events expected on failure.");
    }
}
