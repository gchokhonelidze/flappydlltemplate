namespace Flappybet;

using System.Text;

/**
Example of usage:

    var a = new BonusStrategyFixedProbability(new BonusStrategyFixedProbability.BonusInfoDto
    {
        HE = 0.027m,
        BonusProbability = 100m,
        BonusReserve = 0.5m,
        NumberOfOutcomes = 13,
        Hash = Guid.NewGuid().ToString("N")
    });
    a.Print();
*/

public class BonusStrategyFixedProbability(BonusStrategyFixedProbability.BonusInfoDto dto)
{
	public class BonusInfoDto
	{
		/// <summary>Current seed's hash to calculate result from</summary>
		public required string Hash { get; init; }

		/// <summary>House edge</summary>
		public required decimal HE { get; init; }

		/// <summary>Chance to get bonus in every roll</summary>
		public required decimal BonusProbability { get; init; }

		/// <summary>How much to reserve in order to later give bonus</summary>
		public required decimal BonusReserve { get; init; }

		/// <summary>Number of possible outcomes</summary>
		public required uint NumberOfOutcomes { get; init; }
	}

	public void Print()
	{
		var sb = new StringBuilder();
		sb.Append("BonusMultiplier: ");
		sb.Append(BonusMultiplier());
		sb.Append(", \nIsBonusRoll: ");
		sb.Append(IsBonusRoll() ? 1 : 0);
		sb.Append(", \nPayoutWithBonus: ");
		sb.Append(PayoutWithBonus());
		sb.Append(", \nPayoutWithoutBonus: ");
		sb.Append(PayoutWithoutBonus());
		sb.Append("x");
		sb.Append(", \nPayoutNoBonusEst: ");
		sb.Append(PayoutStandard());
		sb.Append("x");
		Console.WriteLine(sb.ToString());
	}

	public decimal BonusMultiplier()
	{
		return 1 + (100m / dto.BonusProbability * (dto.NumberOfOutcomes * dto.BonusReserve)) / (1 - dto.BonusReserve);
	}

	public bool IsBonusRoll()
	{
		if (dto.BonusReserve == 0)
			return false;
		if (dto.BonusProbability == 0)
			return false;
		var maxRoll = (int)((100m / dto.BonusProbability) * 100000);
		var roll = Rng.Rand(dto.Hash, maxRoll);
		return roll < (maxRoll / (100m / dto.BonusProbability));
	}

	/// <summary>calculates payout without any bonus implementation</summary>
	decimal PayoutStandard() => dto.NumberOfOutcomes * (1 - dto.HE);

	public decimal PayoutWithoutBonus() => PayoutStandard() * (1 - dto.BonusReserve);

	public decimal PayoutWithBonus() => PayoutWithoutBonus() * (IsBonusRoll() ? BonusMultiplier() : 1);
}
