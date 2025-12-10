namespace Flappybet;

public partial class Game : IGame
{
	public static OnBetCustomCheckResultDto OnBetCustomCheck(OnBetCustomCheckDto input)
	{
		return new OnBetCustomCheckResultDto { Result = false };
	}
}
