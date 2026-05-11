using MessagePack;

namespace Flappybet;

[MessagePackObject(true)]
public record PartialState(Dictionary<string, object> Changes);

[MessagePackObject(true)]
public record FullState(Dictionary<string, object> State);

[MessagePackObject(true)]
public record PlayerIndStateChange(long PlayerId, PartialState IndStatePartial);

[MessagePackObject(true)]
public record ChipOp(string? Name, long PlayerId, decimal Delta, decimal BalanceAfter, long Timestamp);

[MessagePackObject(true)]
public record Dealer(int SeatId);

[MessagePackObject(true)]
public record Turn(int SeatId);

[MessagePackObject(true)]
public record Bet(string BetId, long Amount);

[MessagePackObject(true)]
public record Custom(string EventName, long PlayerId, Bet[]? Bets, Dictionary<string, object> CustomData, long Ms);

[MessagePackObject(true)]
public record TakenSeat(int SeatId, long PlayerId, long Ms);

[MessagePackObject(true)]
public record PlayerData(TakenSeat Seat, FullState? IndState, decimal Balance, ChipOp? LastOp, bool IsBot = false);

[MessagePackObject(true)]
public record OnMultiTurnDto(
	string Hash,
	string TypeName, // Table type (MultiTableType.TypeName)
	int MaxThinkDurationMs,
	int MinPlayers,
	int MaxPlayers,
	int TurnIndex,
	long[] TurnPlayerIds,
	List<ChipOp> PrevOps,
	List<PlayerData> Players,
	FullState? PubState,
	FullState? PvtState,
	Dealer? Dealer,
	Turn? Turn,
	Custom? Custom,
	long Ms
);

[MessagePackObject(true)]
public record OnMultiTurnResDto(
	ErrorDto? Error,
	Dictionary<long, ChipOp> Ops, // all chip related ops happened during the turn
	PartialState SPubPartial,
	PartialState SPvtPartial,
	Dictionary<long, PlayerIndStateChange> IndStateChanges,
	long[] TurnPlayerIds,
	int? OverrideThinkDurationMs,
	int? OverrideDelayBeforeNextTurnMs,
	bool RoundEnd,
	Dealer? NextDealer,
	Turn? NextTurn
);

[MessagePackObject(true)]
public record MultiTableType(string TypeName, Dictionary<string, object> Config);
