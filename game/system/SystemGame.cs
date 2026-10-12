namespace Flappybet;

public partial class Game
{
	public static byte[] OnInitSys(byte[] input) => OnInit(input.FromBytes<OnInitDto>()).ToBytes();

	public static byte[] OnBetCustomCheckSys(byte[] input) => OnBetCustomCheck(input.FromBytes<OnBetCustomCheckDto>()).ToBytes();

	public static byte[] OnBetSys(byte[] input) => OnBet(input.FromBytes<OnBetDto>()).ToBytes();

	public static byte[] OnBetIncreaseCheckSys(byte[] input) => OnBetIncreaseCheck(input.FromBytes<OnBetIncreaseCheckDto>()).ToBytes();

	public static byte[] OnCustomSys(byte[] input) => OnCustom(input.FromBytes<OnCustomDto>()).ToBytes();

	public static byte[] OnConfigSys(byte[] _) => OnConfig().ToBytes();

	public static byte[] OnVerifySys(byte[] input) => OnVerify(input.FromBytes<OnVerifyDto>()).ToBytes();

	public static byte[] OnMonteCarloSys(byte[] input) => OnMonteCarlo(input.FromBytes<OnMonteCarloDto>()).ToBytes();

	//Shared:
	public static byte[] OnSharedDataSys(byte[] input) => OnSharedData(input.FromBytes<OnSharedDataDto>()).ToBytes();

	public static byte[] OnTableInitSys(byte[] input) => OnTableInit(input.FromBytes<OnTableInitDto>()).ToBytes();

	public static byte[] OnTableTickSys(byte[] input) => OnTableTick(input.FromBytes<OnTableTickDto>()).ToBytes();

	//Multi:
	public static byte[] OnMultiTurnSys(byte[] input) => OnMultiTurn(input.FromBytes<OnMultiTurnDto>()).ToBytes();

	//Universal:
	public static byte[] OnUniversalInitSys(byte[] input) => OnUniversalInit(input.FromBytes<OnUniversalInitDto>()).ToBytes();

	public static byte[] OnUniversalCanBetSys(byte[] input) => OnUniversalCanBet(input.FromBytes<OnUniversalCanBetDto>()).ToBytes();

	public static byte[] OnUniversalCanRollbackSys(byte[] input) => OnUniversalCanRollback(input.FromBytes<OnUniversalCanRollbackDto>()).ToBytes();

	public static byte[] OnUniversalTickSys(byte[] input) => OnUniversalTick(input.FromBytes<OnUniversalTickDto>()).ToBytes();
}
