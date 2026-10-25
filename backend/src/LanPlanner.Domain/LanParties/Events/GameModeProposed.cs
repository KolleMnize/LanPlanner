using LanPlanner.Domain.Games;
using LanPlanner.Domain.Users;

namespace LanPlanner.Domain.LanParties.Events;

public sealed record GameModeProposed(LanPartyId LanPartyId, UserId ProposedBy, GameModeId ProposedGameMode);
