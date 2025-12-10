using System.Security.Cryptography;
using System.Text;
using MessagePack;
using Newtonsoft.Json;

namespace Flappybet;

public static class Extensions
{
	/// <summary>Convert object to messagepack bytes array</summary>
	public static byte[] ToBytes(this object o) => MessagePackSerializer.Serialize(o);

	/// <summary>Convert messagepack bytes array back to the object</summary>

	public static T FromBytes<T>(this byte[] bytes) => MessagePackSerializer.Deserialize<T>(bytes);

	/// <summary>Convert string to bytes array</summary>
	public static byte[] StringToBytes(this string str) => Encoding.UTF8.GetBytes(str);

	/// <summary>Convert bytes array to string</summary>

	public static string BytesToString(this byte[] bytes)
	{
		var sb = new StringBuilder();
		foreach (byte b in bytes)
			sb.Append(b.ToString("x2"));
		return sb.ToString();
	}

	/// <summary>Convert object to dictionary using messagepack</summary>
	public static Dictionary<string, object> ToDict(this object o) => o.ToBytes().FromBytes<Dictionary<string, object>>();

	/// <summary>Convert dictionary to custom object using messagepack</summary>
	public static T ToCustomDto<T>(this Dictionary<string, object> o) => o.ToBytes().FromBytes<T>();

	/// <summary>Convert dictionary to custom object using messagepack: (ErrorDto? err, MyDto dto) = input.CustomData.ToCustomDtoCatch<MyDto>();</summary>
	public static (ErrorDto?, T) ToCustomDtoCatch<T>(this Dictionary<string, object>? o)
	{
		try
		{
			if (o is null)
				throw new Exception("got_empty_input");
			return (null, o.ToBytes().FromBytes<T>());
		}
		catch (Exception ex)
		{
			return (new ErrorDto { Message = "invalid_arguments", Description = ex.Message }, default(T)!);
		}
	}

	public static async Task<(ErrorDto?, T?)> Catch<T>(this Task<T> task)
	{
		ErrorDto? errorDto = null;
		try
		{
			return (errorDto, await task);
		}
		catch (Exception ex)
		{
			errorDto = new ErrorDto { Message = ex.InnerException?.Message ?? ex.Message ?? "Unexpected error", };
			return (errorDto, default(T));
		}
	}

	public static T RandomPick<T>(this IEnumerable<T> arr, string hash)
	{
		var index = Rng.Rand(hash, arr.Count());
		return arr.ElementAt(index);
	}

	public static void Dump(this object T) => Console.WriteLine(JsonConvert.SerializeObject(T));

	public static void Dump(this object[] T) => Console.WriteLine(JsonConvert.SerializeObject(T));

	public static decimal Truncate(this decimal d, int decimalPoints)
	{
		var v = (decimal)Math.Pow(10, decimalPoints);
		return Decimal.Truncate(d * v) / v;
	}

	public static decimal? NullIfZero(this decimal d) => d == 0 ? null : d;

	public static decimal? NullIfZero(this decimal? d) => d == 0 ? null : d;

	public static int? NullIfZero(this int d) => d == 0 ? null : d;

	public static int? NullIfZero(this int? d) => d == 0 ? null : d;

	public static string StringJoin(this IEnumerable<string> arr, char separator) => string.Join(separator, arr);

	public static string StringJoin(this IEnumerable<string> arr, string separator) => string.Join(separator, arr);

	public static byte[] Sha1(this string s) => SHA1.HashData(s.ToBytes());

	public static byte[] Sha256(this string s) => SHA256.HashData(s.ToBytes());

	public static byte[] Sha512(this string s) => SHA512.HashData(s.ToBytes());

	public static byte[] Md5(this string s) => MD5.HashData(s.ToBytes());

	public static byte[] HmacSha1(this string s, string key) => HMACSHA1.HashData(key.ToBytes(), s.ToBytes());

	public static byte[] HmacSha256(this string s, string key) => HMACSHA256.HashData(key.ToBytes(), s.ToBytes());

	public static byte[] HmacSha384(this string s, string key) => HMACSHA384.HashData(key.ToBytes(), s.ToBytes());

	public static byte[] HmacSha512(this string s, string key) => HMACSHA512.HashData(key.ToBytes(), s.ToBytes());
}
