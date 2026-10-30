using MessagePack;

namespace Flappybet;

[MessagePackObject(true)]
public class OnInitDto
{
	public required decimal HouseEdge { get; set; }
	public PublicStateDto? SPub { get; set; } //prev round state
	public PrivateStateDto? SPvt { get; set; } //prev round state
	public long Ms { get; set; }
}

[MessagePackObject(true)]
public class OnInitResultDto
{
	public required PublicStateDto SPub { get; set; }
}

/// <summary>
/// Some games can have multi BetId, ex. roulette has reds, blacks, zero etc.
/// </summary>
[MessagePackObject(true)]
public class BetDto
{
	public required string Id { get; init; }
	public required decimal Amount { get; init; }
}

[MessagePackObject(true)]
public class OnBetDto
{
	public required BetDto[] Bets { get; set; } = null!;
	public required string Hash { get; set; } = null!; //hmac512
	public required decimal HouseEdge { get; set; }
	public required decimal RateUsd { get; set; }
	public required decimal TotalBetUsd { get; set; }
	public required EBetType Type { get; set; }
	public required PublicStateDto SPub { get; set; }
	public PrivateStateDto? SPvt { get; set; }
	public Dictionary<string, object>? CustomData { get; set; }
	public required long Ms { get; set; }

	/// <summary>
	/// The player's integrator settings for this game and coin, already merged by the host (per game, then per coin, then per coin
	/// per game, the narrower winning key by key). Free-form, so read keys defensively. Null when the integrator set none, and
	/// always null on hosts older than this field, so a game must behave sensibly without it.
	/// </summary>
	public Dictionary<string, object>? IntegratorSettings { get; set; }
}

[MessagePackObject(true)]
public struct PayoutDto
{
	public required string Id { get; set; }
	public required decimal Payout { get; set; }
}

[MessagePackObject(true)]
public class OnBetResultDto
{
	public required Dictionary<string, object> SPubPartial { get; set; }
	public required PrivateStateDto SPvt { get; set; }
	public PayoutDto[]? Payouts { get; set; }

	/// <summary>Custom data that is relevant to this transaction. Only relevant when Payouts are filled. It is stored inside database, and is shown in history!</summary>
	public Dictionary<string, object>? Info { get; set; }

	/// <summary>Last played in game nonce</summary>
	public int? InGameN { get; set; }
	public InGameFreespinDto? Freespins { get; set; }

	/// <summary>Force player to make same amount bet on next round (can be other coin)</summary>
	public bool ForceBetRepeat { get; set; }
	public ErrorDto? Error { get; set; }

	/// <summary>Override default wager calculation. Used in case you have virtual balance.</summary>
	public decimal? Wager { get; set; }
}

[MessagePackObject(true)]
public class OnBetIncreaseCheckDto
{
	public required BetDto[] Bets { get; set; } = null!;
	public required BetDto BetIncrease { get; set; }
	public required decimal HouseEdge { get; set; }
	public required decimal RateUsd { get; set; }
	public required decimal TotalBetUsd { get; set; }
	public required PublicStateDto SPub { get; set; }
	public required PrivateStateDto SPvt { get; set; }
	public required IndStateDto SInd { get; set; }
	public Dictionary<string, object>? Custom { get; set; }
}

[MessagePackObject(true)]
public class OnBetIncreaseCheckResultDto
{
	public required bool Result { get; set; }
}

[MessagePackObject(true)]
public class OnBetCustomCheckDto
{
	public required BetDto[] Bets { get; set; } = null!;
	public required decimal HouseEdge { get; set; }
	public required EBetType Type { get; set; }
	public required Dictionary<string, object> SPub { get; set; }
	public Dictionary<string, object>? SPvt { get; set; } //prev state
	public Dictionary<string, object>? CustomData { get; set; }
	public required long Ms { get; set; }
}

[MessagePackObject(true)]
public class OnBetCustomCheckResultDto
{
	public required bool Result { get; set; }
	public string? Message { get; set; }
}

[MessagePackObject(true)]
public class ErrorDto
{
	public bool Error { get; set; } = true;
	public required string Message { get; set; }
	public string? Description { get; set; }
}

[MessagePackObject(true)]
public class OnCustomDto
{
	///<summary>Initial bets, do not include betincreases</summary>
	public required BetDto[] Bets { get; set; } = null!;

	///<summary>All bet increases including current</summary>
	public BetDto[]? BetIncreases { get; set; }
	public BetDto? BetIncreaseCurrent { get; set; }
	public required decimal HouseEdge { get; set; }
	public required decimal RateUsd { get; set; }
	public required decimal TotalBetUsd { get; set; }
	public required string EventName { get; set; }
	public required Dictionary<string, object> CustomData { get; set; }
	public required PublicStateDto SPub { get; set; }
	public required PrivateStateDto SPvt { get; set; }
	public required long Ms { get; set; }
}

[MessagePackObject(true)]
public class OnCustomResultDto
{
	public required Dictionary<string, object> SPubPartial { get; set; }
	public required Dictionary<string, object> SPvtPartial { get; set; }
	public PayoutDto[]? Payouts { get; set; }

	/// <summary>Custom data that is relevant to this transaction. Only relevant when Payouts are filled. It is stored inside database, and is shown in history!</summary>
	public Dictionary<string, object>? Info { get; set; }

	/// <summary>Force player to make same amount bet on next round (can be other coin)</summary>
	public bool ForceBetRepeat { get; set; }

	/// <summary>Last played in game nonce</summary>
	public int? InGameN { get; set; }
	public InGameFreespinDto? Freespins { get; set; }
	public ErrorDto? Error { get; set; }
}

[MessagePackObject(true)]
public class FreespinInfoDto
{
	///<summary>This betId must only be used with the freespins</summary>
	public string FsBetId { get; set; } = null!;

	///<summary>CustomData that is relevant with this FsBetId</summary>
	public Dictionary<string, object>? CustomData { get; set; }
}

[MessagePackObject(true)]
public class BonusBuyInfoDto
{
	///<summary>Bonus buy bet id</summary>
	public string BonusBuyBetId { get; init; } = null!;

	///<summary>This multiplier is multiplied on related bet level</summary>
	public decimal Multiplier { get; init; }

	///<summary>CustomData that is relevant with this BonusBuyBetId</summary>
	public Dictionary<string, object>? CustomData { get; set; }
}

[MessagePackObject(true)]
public class OnConfigResultDto
{
	public required long GameId { get; init; }

	/// <summary>Valid in game betids. Bet cannot be made if not present here. These do not include freespin betids</summary>
	public required string[] BetIds { get; init; }
	public required FreespinInfoDto[] FreespinInfo { get; init; }
	public required EGameType GameType { get; init; }
	public required MonteCarloInfoDto[] MonteCarloInfo { get; init; }
	public required string Version { get; init; }

	/// <summary>If set to true game have to be adjusted to frame in which it is loaded</summary>
	public required bool ControlledSize { get; init; } = false;
	public required bool BattleReady { get; init; } = false;
	public required int TicksIntervalMs { get; init; } //SHARED
	public required int PrepareDurationMs { get; init; } //SHARED
	public IEnumerable<MultiTableType>? MultiConfigs { get; init; } //MULTI
	public BonusBuyInfoDto[]? BonusBuyInfo { get; init; }

	/// <summary>
	/// What the game accepts besides a plain bet, for the host's DLL security test only: nothing at runtime reads it. Optional, and a
	/// game without one plays exactly as before; the test then only sends generic and malformed events, warns about shallow coverage
	/// and lists every event name it found that the game acts on. With one, the test plays the game through it and holds the game to
	/// it: an event, a value or a moment it accepts but did not declare fails the test, and a failed build cannot be activated.
	/// </summary>
	public GameManifestDto? Manifest { get; init; }
}

[MessagePackObject(true)]
public class GameManifestDto
{
	/// <summary>Custom data a BET may carry, per bet id; null means a bet carries none. The test builds valid bets from it</summary>
	public Dictionary<string, GameFieldDto[]>? BetCustom { get; init; }

	/// <summary>Every custom event the game accepts. Nothing else may change a state or pay</summary>
	public GameActionDto[] Actions { get; init; } = [];

	/// <summary>
	/// Set when the game pays some prizes as fixed amounts (a jackpot in usd), so a payout multiplier legitimately depends on the stake.
	/// The test's stake check then warns instead of failing, and the reviewer reads why. Leave it false otherwise: a multiplier that
	/// moves with the stake is the classic way to rig against big bets.
	/// </summary>
	public bool StakeDependentPayouts { get; init; }

	/// <summary>
	/// The state keys the game writes when it refuses an action: an acknowledgement so the client stops retrying (a "REJECTED" status).
	/// Only while the player's result carries an Error does the test leave these out of what it compares; any other change, and every
	/// payout, still counts as acting on the refused action. A Key ending in "*" matches every key that starts with what comes before it.
	/// </summary>
	public GameStateKeyDto[]? RejectionAck { get; init; }

	/// <summary>
	/// Universal games only, optional. The state keys where your game shows the result: the dice, the winning ids, a player's win.
	/// The test checks that these keys do not change while players can still bet. If one does, the report names it.
	/// <para>
	/// Example, a dice game: Public "dice" is 0 when the round opens and you write 4 only after betting closes, which passes.
	/// Writing 4 while bets are still open fails with "Public dice, declared as carrying the outcome, changed while bets were open".
	/// </para>
	/// <para>
	/// Without this list the leak is still caught (the test plays every round again with another hash and compares what players see),
	/// the report is just less clear. Private keys are ignored, players never see the private state. A key ending in "*" matches every
	/// key that starts with it: "dice*" matches "dice1" and "dice2".
	/// </para>
	/// </summary>
	public GameStateKeyDto[]? OutcomeKeys { get; init; }
}

[MessagePackObject(true)]
public class GameStateKeyDto
{
	public required EGameStateScope Scope { get; init; }

	/// <summary>Top level key of that state; a trailing "*" makes it a prefix</summary>
	public required string Key { get; init; }
}

[MessagePackObject(true)]
public class GameActionDto
{
	/// <summary>The EventName, exactly as the client sends it</summary>
	public required string EventName { get; init; }

	/// <summary>The CustomData fields; empty for an event that carries none</summary>
	public GameFieldDto[] Fields { get; init; } = [];

	/// <summary>When it is allowed: every condition must hold (AND). Empty means always. List several values in one condition for OR</summary>
	public GameStateConditionDto[] When { get; init; } = [];

	/// <summary>Needs an open bet of the player first (a cash out, a reveal)</summary>
	public bool RequiresBet { get; init; }

	/// <summary>How often a player may send it per round; null means no limit (a cash out is 1)</summary>
	public int? MaxPerRound { get; init; }
}

[MessagePackObject(true)]
public class GameStateConditionDto
{
	/// <summary>Which state the key is read from</summary>
	public required EGameStateScope Scope { get; init; }

	/// <summary>Top level key of that state</summary>
	public required string Key { get; init; }

	/// <summary>The values in which the action is allowed, compared as text ("betting", "true", "3")</summary>
	public required string[] Values { get; init; }
}

/// <summary>
/// Public: the round's or table's public state (a single game's SPub). Ind: the acting player's own state. Private: the round's or
/// table's private state (a single game's SPvt). Never reorder: travels as its number
/// </summary>
public enum EGameStateScope
{
	Public,
	Ind,
	Private,
}

[MessagePackObject(true)]
public class GameFieldDto
{
	public required string Name { get; init; }
	public required EGameFieldType Type { get; init; }
	public bool Required { get; init; } = true;

	/// <summary>Int and Decimal, and each item of IntArray and DecimalArray: inclusive range</summary>
	public decimal? Min { get; init; }
	public decimal? Max { get; init; }

	/// <summary>String, and each item of StringArray: the only values allowed; null means any up to MaxLength</summary>
	public string[]? Values { get; init; }
	public int? MaxLength { get; init; }

	/// <summary>Any array type: how many items, and whether they must be distinct</summary>
	public int? MinItems { get; init; }
	public int? MaxItems { get; init; }
	public bool Distinct { get; init; }

	/// <summary>Object, and each item of ObjectArray: the nested fields</summary>
	public GameFieldDto[]? Fields { get; init; }

	/// <summary>
	/// What a String (or each item of a StringArray) holds when it has to match the round rather than a fixed list. The test then builds
	/// a valid value from the round it is playing and invalid ones of its own, so no round id or amount is ever written into the manifest
	/// </summary>
	public EGameFieldFormat Format { get; init; }

	/// <summary>
	/// When this field may be sent at all: every condition must hold on the fields beside it. Empty means always. For a field that only
	/// makes sense with some values of another, e.g. "bonusBuy" only on ordinary difficulties:
	/// <code>When = [new() { Field = "difficulty", Values = ["easy", "medium", "hard"] }]</code>
	/// The test then only builds valid data that keeps to it, and also sends the field where a condition fails, which the game has to
	/// refuse. Required applies only while it holds
	/// </summary>
	public GameFieldConditionDto[]? When { get; init; }
}

/// <summary>A condition on a sibling field of the same data: present, and one of the values</summary>
[MessagePackObject(true)]
public class GameFieldConditionDto
{
	/// <summary>Name of a field beside this one, at the same level</summary>
	public required string Field { get; init; }

	/// <summary>The values in which the field is allowed, compared as text ("easy", "true", "3")</summary>
	public required string[] Values { get; init; }
}

/// <summary>Never reorder: travels as its number</summary>
public enum EGameFieldFormat
{
	/// <summary>Any string within Values / MaxLength</summary>
	None,

	/// <summary>The id of the round being played</summary>
	RoundId,

	/// <summary>One of the bet ids the game declares in OnConfig.BetIds</summary>
	BetId,

	/// <summary>The transaction id of one of the acting player's own bets in the round</summary>
	TransactionId,

	/// <summary>A decimal written as text ("12.5"), within Min and Max</summary>
	DecimalString,
}

/// <summary>Never reorder: travels as its number</summary>
public enum EGameFieldType
{
	Int,
	Decimal,
	String,
	Bool,
	IntArray,
	DecimalArray,
	StringArray,
	BoolArray,
	Object,
	ObjectArray,
}

[MessagePackObject(true)]
public class InGameFreespinDto
{
	public required string BetId { get; set; }
	public required decimal Amount { get; set; }
	public required int Quantity { get; set; }
}

[MessagePackObject(true)]
public class OnVerifyDto
{
	public required BetDto[] Bets { get; init; }
	public required string Hash { get; init; } = null!; //hmac512
	public required decimal HouseEdge { get; set; }
	public Dictionary<string, object>? CustomData { get; init; }

	/// <summary>Last played in game nonce</summary>
	public int? InGameNonce { get; init; }

	/// <summary>If true, you must return all outcomes from 1 to last provided In Game Nonce, otherwise only last outcome</summary>
	public bool? All { get; init; }
}

[MessagePackObject(true)]
public class OnVerifyResultDto
{
	public required Dictionary<string, object> Outcome { get; set; }
}

[MessagePackObject(true)]
public record MonteCarloInfoDto
{
	public string McBetId { get; set; } = null!;
	public Dictionary<string, object>? CustomData { get; set; }
}

[MessagePackObject(true)]
public record OnMonteCarloDto
{
	public required string BetId { get; init; }
	public required decimal AmountUsd { get; init; }
	public required string Hash { get; set; } = null!; //hmac512
	public required decimal HouseEdge { get; set; }
	public Dictionary<string, object>? CustomData { get; set; }
}

[MessagePackObject(true)]
public record OnMonteCarloResultDto
{
	public required decimal TotalBetUsd { get; init; }
	public required decimal GrossWinUsd { get; init; }
}

[MessagePackObject(true)]
public class DllLogEntry
{
	public string? Level { get; set; } //DEBUG / INFO / WARNING / ERROR
	public string? Message { get; set; }
	public string? Method { get; set; }
	public string? Detail { get; set; }
	public Dictionary<string, object>? Data { get; set; }
	public long? DurationMs { get; set; }
}
