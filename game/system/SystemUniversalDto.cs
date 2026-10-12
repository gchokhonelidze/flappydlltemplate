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

/// <summary>
/// One bet a player placed in the round. <see cref="IntegratorSettings"/> is the player's integrator settings as they stood when
/// the bet was placed, merged by the host the same way as <see cref="OnBetDto.IntegratorSettings"/>. It rides on the bet because one
/// round mixes players of several integrators and coins. Null when the integrator set none, or on hosts older than this field.
/// </summary>
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
	long Ms,
	Dictionary<string, object>? IntegratorSettings = null
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
/// <para>
/// <see cref="IntegratorSettings"/> is this player's integrator settings, the same bag the bet will carry into the round once it is
/// accepted, so a refusal can depend on them. Null when the integrator set none, or on hosts older than this field.
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
	long Ms,
	Dictionary<string, object>? IntegratorSettings = null
);

/// <summary>
/// Refusing stops the bet before the integrator is called, so the player is never charged for it. <see cref="Error"/> is what the
/// player is told; leaving it null falls back to a generic refusal.
/// </summary>
[MessagePackObject(true)]
public record OnUniversalCanBetResultDto(bool CanBet, ErrorDto? Error);

/// <summary>
/// Asked when a player wants a bet of theirs back: the game decides whether it may still be taken out of the round as it stands right
/// now. It carries the round the same way <see cref="OnUniversalCanBetDto"/> does, so the answer can depend on the phase the round is
/// in, on the player's other bets and on what the outcome is going to be.
/// <para>
/// Only ever asked for a bet nothing has been paid on yet: a freespin, or a bet a tick already put a payout on, is refused by the host
/// before the game is asked. Allowing it takes the bet out of the round and gives the player their stake back. Nothing else tells the
/// game about it: the next tick simply no longer has the bet in <see cref="OnUniversalTickDto.Bets"/>, so a game that keeps bets in its
/// own state has to notice the bet is gone from there. A fully paid bet leaves that list too, so the way to tell them apart is that a
/// bet taken back always leaves with none of its items paid. A payout a tick still returns for it lands nowhere.
/// </para>
/// <para>
/// <see cref="MyBets"/> is everything this player holds in the round, keyed by transaction id, <see cref="Bet"/> included.
/// </para>
/// <para>
/// <see cref="IntegratorSettings"/> is this player's integrator settings as they stand now. The bet carries its own copy from when it was
/// placed. Null when the integrator set none.
/// </para>
/// <para>
/// The host cannot tell whether betting is over or the result is already shown. Only your game knows what its state means, so that
/// is your job: look at the states you are given here, and if they say the bet should not be taken back any more (betting has ended,
/// the result is visible), refuse.
/// <para>
/// You do not need to worry about the round moving on while you answer. If a tick changes any state after you said yes, the host
/// does not use your yes. It asks you again with the new states, and you answer again.
/// </para>
/// Example: you see <c>phase = "betting"</c> and allow. A tick sets <c>phase = "rolled"</c> before the bet is removed. The host notices
/// the change and asks again, you now see <c>"rolled"</c> and refuse. The bet stays.
/// </para>
/// <para>
/// A game that does not implement this route never has a bet taken back: the host reads a missing route as a refusal.
/// </para>
/// </summary>
[MessagePackObject(true)]
public record OnUniversalCanRollbackDto(
	string RoundId,
	string Hash,
	decimal HouseEdge,
	long PlayerId,
	FullState PublicState,
	FullState PrivateState,
	FullState? IndState,
	UniversalBetDto Bet,
	Dictionary<UniversalTransactionId, UniversalBetDto> MyBets,
	long Ms,
	Dictionary<string, object>? IntegratorSettings = null
);

/// <summary>
/// Refusing leaves the bet in the round exactly as it was and nothing is refunded. <see cref="Error"/> is what the player is told;
/// leaving it null falls back to a generic refusal.
/// </summary>
[MessagePackObject(true)]
public record OnUniversalCanRollbackResultDto(bool CanRollback, ErrorDto? Error);

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
/// <param name="TickNonce">
/// Counts the ticks of the game's loop: 1, 2, 3... It does not start over with a new round, but it does start over at 1 when the loop
/// is restarted (a deploy, a crash). Good for logs, not for game logic.
/// </param>
/// <param name="RoundId">Id of the round being ticked. It stays the same for every tick of the round, a new round gets a new one.</param>
/// <param name="Hash">
/// The round's hash, made from its server seed. Same value on every tick of the round and in the init that opened it, so draw the
/// outcome from this and it stays verifiable.
/// </param>
/// <param name="HouseEdge">House edge of the round, in percent. Fixed when the round opens; an admin change only applies to the next round.</param>
/// <param name="PublicState">What every player sees. Return changes in <see cref="OnUniversalTickResultDto.PublicState"/> and they go out to everyone.</param>
/// <param name="PrivateState">What nobody sees: keep the outcome here until it is time to reveal it.</param>
/// <param name="IndStates">
/// One state per player, seen only by that player. A player appears here from their first bet or custom in the round, starting with
/// an empty state, so every player who did anything this round is in here even if they have no bet.
/// </param>
/// <param name="Bets">
/// Every bet in the round that still has something left to pay, from every player, not only the new ones. A bet keeps coming back
/// on every tick until all of its items are paid.
/// <list type="bullet">
/// <item>A new bet has a null <see cref="UniversalBetItemDto.Payout"/> on all of its items.</item>
/// <item>A bet you paid only part of comes back with the paid items filled in. Paying those again does nothing.</item>
/// <item>A bet stops coming back once every item is paid. It is done: its win goes out and it still shows up in the round history.</item>
/// <item>A bet also stops coming back if the player took it back (see <see cref="OnUniversalCanRollbackDto"/>).</item>
/// </list>
/// So this is not a list of everything that happened in the round. If the game needs that (a running total, a leaderboard), keep it
/// in a state.
/// Group by <see cref="UniversalBetDto.PlayerId"/> to get one player's bets. To pay one, return it in
/// <see cref="UniversalPlayerResultDto.Payouts"/> under its <see cref="UniversalBetDto.TransactionId"/>.
/// </param>
/// <param name="Customs">
/// Custom actions players sent since the last tick, at most one per player. You only see each one once, on this tick, so write
/// whatever it changes into a state or a payout now.
/// </param>
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
