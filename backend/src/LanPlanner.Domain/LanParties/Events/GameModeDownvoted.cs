using LanPlanner.Domain.Games;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record class GameModeDownvoted(LanPartyId LanPartyId, UserId UserId, GameModeId GameModeId);