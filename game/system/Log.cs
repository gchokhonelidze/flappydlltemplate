using MessagePack;

namespace Flappybet;

public static class Log
{
	private static Action<byte[]>? sink;

	public static void Set(Action<byte[]> log) => sink = log; //host calls this on load

	public static bool Enabled => sink is not null;

	public static void Debug(string message, string? method = null, Dictionary<string, object>? data = null, long? durationMs = null) =>
		Write("DEBUG", message, method, data, durationMs: durationMs);

	public static void Info(string message, string? method = null, Dictionary<string, object>? data = null, long? durationMs = null) =>
		Write("INFO", message, method, data, durationMs: durationMs);

	public static void Warning(string message, string? method = null, Dictionary<string, object>? data = null, long? durationMs = null) =>
		Write("WARNING", message, method, data, durationMs: durationMs);

	public static void Error(string message, Exception? ex = null, string? method = null, Dictionary<string, object>? data = null) =>
		Write("ERROR", message, method, data, ex?.ToString());

	private static void Write(
		string level,
		string message,
		string? method,
		Dictionary<string, object>? data,
		string? detail = null,
		long? durationMs = null
	)
	{
		var log = sink;
		if (log is null) //production / logging disabled
			return;
		var entry = new DllLogEntry
		{
			Level = level,
			Message = message,
			Method = method,
			Detail = detail,
			Data = data,
			DurationMs = durationMs
		};
		try
		{
			log(MessagePackSerializer.Serialize(entry));
		}
		catch (Exception)
		{
			//data held something messagepack cannot write as object (decimal, custom class), retry as text:
			try
			{
				entry.Data = data?.ToDictionary(_ => _.Key, _ => (object)(_.Value?.ToString() ?? ""));
				log(MessagePackSerializer.Serialize(entry));
			}
			catch (Exception) { } //logging must never break the game
		}
	}
}
