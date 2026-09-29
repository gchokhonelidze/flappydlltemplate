using MessagePack;

namespace Flappybet;

[MessagePackObject(true)]
public class BetPayoutDto
{
	public required string BetId { get; set; }
	public required decimal Amount { get; set; }
	public required decimal Payout { get; set; }
}

[MessagePackObject(true)]
public class CustomDto
{
	public string EventName { get; set; } = null!; //event based on which we should recast CustomData to our dto
	public Dictionary<string, object> CustomData { get; set; } = null!;
}

[MessagePackObject(true)]
public class PlayerDataDto
{
	public required string PSId { get; set; } //player, currency ...
	public required BetPayoutDto[] BetPayouts { get; set; } = default!;
	public required decimal RateUsd { get; set; }
	public required decimal TotalBetUsd { get; set; }
	public required EBetType Type { get; set; }
	public required IndStateDto SInd { get; set; }

	//---
	public CustomDto? Custom { get; set; }
	public BetDto[]? BetIncreases { get; set; }
	public BetDto? BetIncreaseCurrent { get; set; }

	/// <summary>
	/// This player's integrator settings, merged by the host the same way as <see cref="OnBetDto.IntegratorSettings"/>. It is per
	/// player and not on <see cref="OnTableInitDto"/> because one table mixes players of several integrators and coins. Null when the
	/// integrator set none, or on hosts older than this field.
	/// </summary>
	public Dictionary<string, object>? IntegratorSettings { get; set; }
}

[MessagePackObject(true)]
public class OnTableInitDto
{
	public required string Hash { get; set; }
	public required decimal HouseEdge { get; set; }
	public PublicStateDto? SPub { get; set; } //prev round state
	public PrivateStateDto? SPvt { get; set; } //prev round state
	public required long Ms { get; set; }
}

[MessagePackObject(true)]
public class OnTableInitResultDto
{
	public required PublicStateDto SPub { get; set; }
	public required PrivateStateDto SPvt { get; set; }
	public required IndStateDto SInd { get; set; }
	public required long Ms { get; set; }
}

[MessagePackObject(true)]
public class OnTableTickDto
{
	public required decimal HouseEdge { get; set; }
	public required int TickNonce { get; set; }
	public required PublicStateDto SPub { get; set; }
	public required PrivateStateDto SPvt { get; set; }
	public required ExternalDataDto ExternalData { get; set; }
	public PlayerDataDto[]? Datas { get; set; }
	public required long Ms { get; set; }
}

[MessagePackObject(true)]
public class PlayerDataResultDto
{
	public required string PSId { get; set; } //player, currency ...
	public PayoutDto[]? Payouts { get; set; }
	public required bool End { get; set; }
	public Dictionary<string, object>? SIndPartial { get; set; }
	public InGameFreespinDto? Freespins { get; set; } //assign freespins from dll
	public ErrorDto? Error { get; set; }
}

[MessagePackObject(true)]
public class OnTableTickResultDto
{
	public required Dictionary<string, object> SPubPartial { get; set; }
	public required Dictionary<string, object> SPvtPartial { get; set; }
	public required PlayerDataResultDto[]? Datas { get; set; }
	public required bool IsRoundFinished { get; set; }
	public required long Ms { get; set; }
}

[MessagePackObject(true)]
public record SharedData(long PlayerId, string IPlayerId, string? PlayerName, string? PlayerImage, Dictionary<string, object> Data);

[MessagePackObject(true)]
public record OnSharedDataDto(FullState SShared, IEnumerable<SharedData> Datas);

[MessagePackObject(true)]
public record OnSharedDataResultDto(PartialState SSharedPartial);

[MessagePackObject(true)]
public record BlockTronDto
{
	public required string Hash { get; init; } // '0x000000000468afeb2f9b0fe992d7baff4c9951a6acf7e6cb0e6f81280ea0089b',
	public required string Number { get; init; } // '0x468afeb',
	public required string Timestamp { get; init; } // '0x68760de3'
}

[MessagePackObject(true)]
public record ExternalDataDto
{
	public required BlockTronDto Tron { get; init; }
}
