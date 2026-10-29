namespace Flappybet;

public partial class Game
{
	/// <summary>
	/// Get non repeating random numbers from (n) population
	/// example: RandArr(new string[] {"af","45"}, 20) will return 2 different outcomes between 0 and 19 inclusive
	/// 2 element hex is number between 0-255 inclusive;
	/// 3 element [0-4095] inclusive
	/// 4 element [0-65535] inclusive
	/// 5 element [0-1048575] inclusive
	/// </summary>
	public static int[] RandElements(string[] hexes, int n)
	{
		if (hexes.Length > n)
			throw new Exception("overflow");
		var boxes = new bool[n]; //default false
		var indexes = new int[hexes.Length];
		for (int i = 0; i < hexes.Length; i++)
			indexes[i] = Rng.Rand(hexes[i], n - i);
		foreach (var index in indexes)
		{
			int d = index;
			int i = 0;
			while (true)
			{
				if (boxes[i])
					d++;
				if (i == d)
				{
					boxes[i] = true;
					break;
				}
				i++;
			}
		}
		return boxes.Select((box, i) => (box, i)).Where(tuple => tuple.box).Select(tuple => tuple.i).ToArray();
	}

	public static (int, int) Rand2(string hex1, string hex2, int n)
	{
		int index1 = Rng.Rand(hex1, n);
		int index2 = Rng.Rand(hex2, n - 1);
		index2 = index1 + index2 + 1;
		if (index2 > n - 1)
			index2 -= n;
		return (index1 + 1, index2 + 1);
	}

	/// <summary>Sum of probabilities must be equal to 1, returns index</summary>
	public static int GetRandomOutcome(string hex, decimal[] probabilities)
	{
		var randomValue = Rng.Rand(hex, 1000000) / 1000000m;
		decimal cumulativeProbability = 0;
		for (int i = 0; i < probabilities.Length; i++)
		{
			cumulativeProbability += probabilities[i];
			if (randomValue < cumulativeProbability)
				return i; // Return the index of the outcome
		}
		throw new Exception("unexpected_error");
	}

	/// <summary>returns a!/(b! * (a - b)!) where a>=b</summary>
	public static long BinomialCoefficient(int a, int b)
	{
		if (b < 0 || b > a)
			return 0;
		if (b == 0 || b == a)
			return 1;
		long[,] dp = new long[a + 1, b + 1];
		for (int i = 0; i <= a; i++)
		for (int j = 0; j <= Math.Min(i, b); j++)
		{
			if (j == 0 || j == i)
				dp[i, j] = 1;
			else
				dp[i, j] = dp[i - 1, j - 1] + dp[i - 1, j];
		}
		return dp[a, b];
	}

	/// <summary>Multinomial Coefficient: Ways to distribute duplicate values among dice</summary>
	public static int MultinomialCoefficient(int n, int[] counts)
	{
		int numerator = Factorial(n);
		int denominator = counts.Aggregate(1, (acc, x) => acc * Factorial(x));
		return numerator / denominator;
	}

	public static int Factorial(int n)
	{
		int result = 1;
		for (int i = 2; i <= n; i++)
			result *= i;
		return result;
	}

	/// <summary>Calculate how many combinations possible for certain dice numbers (ordering matters: 1,2,3 is not same as 1,3,2)</summary>
	public static int CalculateCombinations(int diceCount, int sides, int[] fixedNumbers)
	{
		var numberCounts = fixedNumbers.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
		int fixedCount = fixedNumbers.Length; // How many dice are assigned fixed numbers
		int remainingDice = diceCount - fixedCount;
		if (fixedCount > diceCount)
			return 0; // Not possible to fit more fixed numbers than dice
		// Compute how many ways we can assign fixed numbers to dice
		int placementWays = MultinomialCoefficient(diceCount, numberCounts.Values.ToArray());
		// The remaining dice can be any of the other values
		int remainingChoices = sides - numberCounts.Keys.Count;
		int freeDiceWays = (int)Math.Pow(remainingChoices, remainingDice);
		return placementWays * freeDiceWays;
	}
}
