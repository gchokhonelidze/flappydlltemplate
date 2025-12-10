namespace Flappybet;

public interface IGameSingle
{
	public static abstract OnInitResultDto OnInit(OnInitDto input);
	public static abstract OnBetResultDto OnBet(OnBetDto input);
	public static abstract OnCustomResultDto OnCustom(OnCustomDto input);
	public static abstract OnBetIncreaseCheckResultDto OnBetIncreaseCheck(OnBetIncreaseCheckDto input);
	public static abstract OnBetCustomCheckResultDto OnBetCustomCheck(OnBetCustomCheckDto input);
}

public interface IGameShared
{
	public static abstract OnTableInitResultDto OnTableInit(OnTableInitDto input);

	public static abstract OnTableTickResultDto OnTableTick(OnTableTickDto input);
}

public interface IGame : IGameSingle, IGameShared
{
	public static abstract OnConfigResultDto OnConfig();
	public static abstract OnVerifyResultDto OnVerify(OnVerifyDto input);
	public static abstract OnMonteCarloResultDto OnMonteCarlo(OnMonteCarloDto input);
}
