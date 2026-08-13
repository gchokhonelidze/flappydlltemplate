namespace Flappybet;

public interface IGameUniversal
{
	public static abstract OnUniversalInitResultDto OnUniversalInit(OnUniversalInitDto input);
	public static abstract OnUniversalCanBetResultDto OnUniversalCanBet(OnUniversalCanBetDto input);
	public static abstract OnUniversalTickResultDto OnUniversalTick(OnUniversalTickDto input);
}
