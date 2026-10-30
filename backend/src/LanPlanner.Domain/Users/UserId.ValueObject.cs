using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Users
{
    public class UserId : AggregateId
    {
        private UserId(Guid value) : base(value)
        {
        }

        public static ErrorOr<UserId> Create(Guid value)
        {
            if (value == Guid.Empty)
                return UserErrors.UserIdCannotBeEmpty;

            return new UserId(value);
        }
    }
}