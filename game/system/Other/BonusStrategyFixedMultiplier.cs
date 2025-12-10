namespace Flappybet;

using System.Text;

/**
Example of usage:

    var a = new BonusStrategy(
        new BonusStrategy.InfoDto
        {
            HE = 0.027m,
            BonusMultiplier = 20m,
            BonusReserve = 0.0143m,
            NormalPayoutWithoutHouseEdge = 12,
            Hash = Guid.NewGuid().ToString("N")
        }
    );
    a.Print();
*/


public class BonusInfoDto
{
	/// <summary>Current seed's hash to calculate result from</summary>
	public required string Hash { get; init; }

	/// <summary>House edge</summary>
	public required decimal HE { get; init; }

	/// <summary>Multiplies standard payout with this value in case of bonus</summary>
	public required decimal BonusMultiplier { get; init; }

	/// <summary>How much to reserve in order to later give bonus</summary>
	public required decimal BonusReserve { get; init; }

	/// <summary>Your normal payout (no houseedge incl.)</summary>
	public required decimal NormalPayoutWithoutHouseEdge { get; init; }
}

public class BonusStrategyFixedMultiplier(BonusInfoDto dto)
{
	public void Print()
	{
		var sb = new StringBuilder();
		sb.Append("BonusProbability: ");
		sb.Append(BonusProbability());
		sb.Append(", \nIsBonusRoll: ");
		sb.Append(IsBonusRoll() ? 1 : 0);
		sb.Append(", \nPayout: ");
		sb.Append(Payout());
		sb.Append("x");
		sb.Append(", \nPayoutNoBonusEst: ");
		sb.Append(PayoutStandard());
		sb.Append("x");
		Console.WriteLine(sb.ToString());
	}

	public decimal BonusProbability()
	{
		if (dto.BonusReserve == 0)
			return 0;
		decimal bonusProbability = (dto.BonusMultiplier - 1) * (1 - dto.BonusReserve) / (dto.NormalPayoutWithoutHouseEdge * dto.BonusReserve);
		return bonusProbability.Truncate(3);
	}

	public bool IsBonusRoll()
	{
		if (dto.BonusReserve == 0)
			return false;
		var bonusProbability = BonusProbability();
		if (bonusProbability == 0)
			return false;
		var maxRoll = (int)(bonusProbability * 100000);
		var roll = Rng.Rand(dto.Hash, maxRoll);
		return roll < (maxRoll / bonusProbability);
	}

	/// <summary>calculates payout without any bonus implementation</summary>
	decimal PayoutStandard() => dto.NormalPayoutWithoutHouseEdge * (1 - dto.HE);

	decimal PayoutWithoutBonus() => PayoutStandard() * (1 - dto.BonusReserve);

	public decimal Payout() => PayoutWithoutBonus() * (IsBonusRoll() ? dto.BonusMultiplier : 1);
}
