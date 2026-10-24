using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.LanParties;

public class LanPartyId : AggregateId
{
    private LanPartyId(Guid value) : base(value)
    {
    }

    public static ErrorOr<LanPartyId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return LanPartyErrors.LanPartyIdCannotBeEmpty;

        return new LanPartyId(value);
    }

}