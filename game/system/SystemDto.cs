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
