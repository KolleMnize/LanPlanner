using ErrorOr;
using LanPlanner.Domain.SharedKernel;

namespace LanPlanner.Domain.Games
{
    public class GameId : AggregateId
    {
        private GameId(Guid value) : base(value)
        {
        }

        public static ErrorOr<GameId> Create(Guid value)
        {
            if (value == Guid.Empty)
                return GameErrors.GameIdCannotBeEmpty;

            return new GameId(value);
        }
    }
}
