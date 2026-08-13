namespace Flappybet;

public partial class Game : IGame
{
	/// <summary>
	/// What one tick does to one player, called by <see cref="OnUniversalTick"/> once per player of the round. It is handed a single
	/// player and knows nothing about the others. <paramref name="custom"/> is the action they are waiting to have dealt with, null
	/// when they sent none, and this tick is the only one that will ever be handed it. Returning null leaves the player as they are.
	/// </summary>
	private static UniversalPlayerResultDto? OnUniversalPlayerTick(OnUniversalTickDto input, UniversalPlayerId playerId, UniversalCustomDto? custom)
	{
		Dictionary<UniversalTransactionId, Dictionary<UniversalBetId, decimal>> payouts = [];
		foreach (var bet in input.Bets.Where(bet => bet.PlayerId == playerId.PlayerId))
		{
			// only what is still open: an item that already carries a payout has been paid, and a bet is only ever settled once
			var open = bet.BetItems.Where(item => item.Value.Payout is null).ToArray();
			if (open.Length == 0)
				continue;
			// keyed by the transaction first, because a player may hold several bets and a bet id alone would not say which one is paid.
			// The value is a multiplier of what was staked on that item, so 0 is a lose. Filling every item of a bet settles it and its
			// win goes out at once, and whatever is left open when the round finishes is settled as a lose
			payouts[bet.TransactionId] = open.ToDictionary(item => item.Key, _ => 0m);
		}
		return new UniversalPlayerResultDto(IndState: null, Payouts: payouts.Count > 0 ? payouts : null, Error: null);
	}
}
