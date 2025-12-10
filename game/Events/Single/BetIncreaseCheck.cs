namespace Flappybet;

public partial class Game : IGame
{
	public static OnBetIncreaseCheckResultDto OnBetIncreaseCheck(OnBetIncreaseCheckDto input)
	{
		return new OnBetIncreaseCheckResultDto { Result = false };
	}
}
