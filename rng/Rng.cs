using System.Security.Cryptography;
using System.Text;
using Menso.Tools.Scrambler;
using MessagePack;

namespace Flappybet;

public static class Rng
{
	public static string BytesToString(byte[] bytes)
	{
		var sb = new StringBuilder();
		foreach (byte b in bytes)
			sb.Append(b.ToString("x2"));
		return sb.ToString();
	}

	public static byte[] ToBytes(object o) => MessagePackSerializer.Serialize(o);

	public static byte[] Sha1(string s) => SHA1.HashData(ToBytes(s));

	public static void ValidateHexLength(string hex, long n)
	{
		long minRequiredLength = n switch
		{
			<= 1 => throw new ArgumentOutOfRangeException(nameof(n)),
			<= 16 => 1,
			<= 256 => 2,
			<= 4096 => 3,
			<= 65536 => 4,
			<= 1048576 => 5,
			<= 16777216 => 6,
			<= 268435456 => 7,
			<= 4294967296 => 8,
			<= 68719476736 => 9,
			<= 1099511627776 => 10,
			<= 17592186044416 => 11,
			<= 281474976710656 => 12,
			<= 4503599627370496 => 13,
			<= 72057594037927936 => 14,
			<= 1152921504606846976 => 15,
			_ => throw new OverflowException(nameof(n))
		};
		if (hex.Length < minRequiredLength)
			throw new ArgumentException($"hex length is not enough to make symmetrical randomization spread for {n} outcomes");
	}

	/// <summary>Returns random int result from (0) to (n-1) inclusive</summary>
	public static int Rand(string hex, int n)
	{
		ValidateHexLength(hex, n);
		hex = hex.Length > 15 ? hex[..15] : hex;
		decimal v = long.Parse(hex, System.Globalization.NumberStyles.HexNumber);
		decimal max = (decimal)Math.Pow(2, hex.Length * 4);
		int random = (int)Math.Floor(v / max * n);
		return random;
	}

	/// <summary>Returns random long result from (0) to (n-1) inclusive</summary>
	public static long RandLong(string hex, long n)
	{
		ValidateHexLength(hex, n);
		hex = hex.Length > 15 ? hex[..15] : hex;
		decimal v = long.Parse(hex, System.Globalization.NumberStyles.HexNumber);
		decimal max = (decimal)Math.Pow(2, hex.Length * 4);
		long random = (long)Math.Floor(v / max * n);
		return random;
	}

	public static string RotateHash(string hash, int nonce) => BytesToString(Sha1($"{hash}{nonce}"));

	/// <summary>Shuffles array using Fisher-Yates algorithm. Does not mutate input array</summary>
	public static IList<T> Shuffle<T>(IList<T> arr, string hash)
	{
		var seed = int.Parse(hash[..8], System.Globalization.NumberStyles.HexNumber);
		IList<T> copy = [.. arr];
		copy.Shuffle(seed);
		return copy;
	}
}
