using MessagePack;

namespace Flappybet;

/// <summary>
/// Bet id of a universal game. It is a key of <see cref="UniversalBetDto.BetItems"/> and of
/// <see cref="UniversalPlayerResultDto.Payouts"/>, so a bet and the payout it earned line up by value.
/// </summary>
[MessagePackObject(true)]
public record UniversalBetId(string BetId);

[MessagePackObject(true)]
public record UniversalPlayerId(long PlayerId);

/// <summary>
/// Identifies one bet a player placed in the round. Every bet the tick hands you carries its own, and it is the key of
/// <see cref="UniversalPlayerResultDto.Payouts"/>, so paying a bet is a matter of keying by the id that came with it.
/// </summary>
[MessagePackObject(true)]
public record UniversalTransactionId(string TransactionId);

/// <summary>
/// The custom action a player is waiting to have dealt with. It is a one shot event, not state: the tick that is handed it is the
/// only one that ever sees it, and it is gone from the round by the time the next tick runs, so anything it is meant to leave behind
/// has to be written into a state or a payout in the result.
/// <para>
/// A player holds one at a time and the first one wins: another sent before a tick has picked this one up is refused and the player
/// is told, so what a tick is handed is the first thing they asked for and nothing they asked for is silently dropped.
/// </para>
/// </summary>
[MessagePackObject(true)]
public record UniversalCustomDto(string EventName, Dictionary<string, object> Custom, long Ms);

/// <summary>One position of a bet. Payout and the time it was paid at both stay null until the round settles it.</summary>
[MessagePackObject(true)]
public record UniversalBetItemDto(UniversalBetId BetId, decimal BetAmount, decimal? Payout, long? PayoutMs);

[MessagePackObject(true)]
public record UniversalBetDto(
	UniversalTransactionId TransactionId,
	long PlayerId,
	string RoundId,
	Dictionary<UniversalBetId, UniversalBetItemDto> BetItems,
	decimal RateUsd,
	long CoinId,
	EBetType Type,
	Dictionary<string, object>? Custom,
	long Ms
);

/// <summary>
/// Opens a round: this is where the game decides what the round starts out as. <see cref="Hash"/> is the round's own hash and is
/// the same one every tick of this round is handed, so an outcome drawn here stays verifiable for the whole round. The previous
/// states are the ones of the round that just finished, and are null for the first round of a provider game.
/// </summary>
[MessagePackObject(true)]
public record OnUniversalInitDto(string RoundId, string Hash, decimal HouseEdge, FullState? PreviousPublicState, FullState? PreviousPrivateState);

[MessagePackObject(true)]
public record OnUniversalInitResultDto(FullState PublicState, FullState PrivateState);

/// <summary>
/// Asked before the player's money is touched: the game decides whether this bet may be placed into the round as it stands right now.
/// It carries the round the way a tick sees it plus the bet being asked for, so the answer can depend on the phase the round is in,
/// on what the player holds already and on what the outcome is going to be.
/// <para>
/// <see cref="MyBets"/> is what this player already has in the round, keyed by transaction id. Universal games put no cap on how many
/// bets a player may place, so a game that wants one is expected to enforce it from here.
/// </para>
/// </summary>
[MessagePackObject(true)]
public record OnUniversalCanBetDto(
	string RoundId,
	string Hash,
	decimal HouseEdge,
	long PlayerId,
	FullState PublicState,
	FullState PrivateState,
	FullState? IndState,
	Dictionary<UniversalBetId, UniversalBetItemDto> BetItems,
	Dictionary<UniversalTransactionId, UniversalBetDto> MyBets,
	Dictionary<string, object>? Custom,
	long Ms
);

/// <summary>
/// Refusing stops the bet before the integrator is called, so the player is never charged for it. <see cref="Error"/> is what the
/// player is told; leaving it null falls back to a generic refusal.
/// </summary>
[MessagePackObject(true)]
public record OnUniversalCanBetResultDto(bool CanBet, ErrorDto? Error);

/// <summary>
/// The round as it stands at the start of a tick. Every provider running the game gets its own round, so this is one provider's
/// round only. The three states differ in who is allowed to see them:
/// <see cref="PublicState"/> reaches every player of the round, <see cref="IndStates"/> reaches only the player it belongs to,
/// and <see cref="PrivateState"/> reaches nobody: it is where the outcome lives until the round reveals it.
/// <para>
/// <see cref="Customs"/> is what players sent since the previous tick. Unlike everything else here it is not state but a one shot
/// queue: this tick is the only one that will ever be handed these, they are dropped from the round as soon as it returns.
/// </para>
/// </summary>
[MessagePackObject(true)]
public record OnUniversalTickDto(
	int TickNonce,
	string RoundId,
	string Hash,
	decimal HouseEdge,
	FullState PublicState,
	FullState PrivateState,
	Dictionary<UniversalPlayerId, FullState> IndStates,
	HashSet<UniversalBetDto> Bets,
	Dictionary<UniversalPlayerId, UniversalCustomDto> Customs
);

/// <summary>
/// What one player takes away from a tick. Every field is optional: leave it null and nothing changes for that player.
/// <para>
/// <see cref="Payouts"/> is keyed by transaction id first, then by bet id, because a player may hold several bets in one round and a
/// bet id on its own would not say which of them is being paid. The value is a multiplier of that item's amount, so 0 is a lose.
/// Paying every item of a bet settles it and its win goes out at once; leave an item out and the bet waits, and whatever is still
/// unpaid when the round finishes is settled as a lose.
/// </para>
/// </summary>
[MessagePackObject(true)]
public record UniversalPlayerResultDto(
	PartialState? IndState,
	Dictionary<UniversalTransactionId, Dictionary<UniversalBetId, decimal>>? Payouts,
	ErrorDto? Error
);

/// <summary>
/// The changes a tick produced. The state changes are partial: only the keys named here are written, the rest of the state is left
/// as it was. Setting <see cref="IsRoundFinished"/> closes the round and the host opens the next one in its place.
/// </summary>
[MessagePackObject(true)]
public record OnUniversalTickResultDto(
	bool IsRoundFinished,
	PartialState? PublicState,
	PartialState? PrivateState,
	Dictionary<UniversalPlayerId, UniversalPlayerResultDto>? PlayerResults
);
