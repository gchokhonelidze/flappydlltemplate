namespace Flappybet;

public partial class Game : IGame
{
	/// <summary>
	/// Optional. Draws one finished round as html, for example the dice and the amount won. The host only calls it once the round has ended.
	/// Empty html means the game has no view, and the host shows the OnVerify outcome as text instead.
	/// </summary>
	public static OnRoundViewResultDto OnRoundView(OnRoundViewDto input)
	{
		return new OnRoundViewResultDto { Html = "" };
	}
}
