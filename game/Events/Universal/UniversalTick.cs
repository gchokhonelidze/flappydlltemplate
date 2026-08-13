namespace Flappybet;

public partial class Game : IGame
{
	/// <summary>
	/// Moves the round forward by one tick and walks the players. Everything that happens to a single player lives in
	/// <see cref="OnUniversalPlayerTick"/>.
	/// </summary>
	public static OnUniversalTickResultDto OnUniversalTick(OnUniversalTickDto input)
	{
		Dictionary<UniversalPlayerId, UniversalPlayerResultDto> playerResults = [];
		// every player who has taken part in the round: the host seats them here the moment they bet or send a custom
		foreach (var playerId in input.IndStates.Keys)
		{
			var playerResult = OnUniversalPlayerTick(input, playerId, input.Customs.GetValueOrDefault(playerId));
			// null means nothing changes for them, and a player with no entry is left exactly as they were
			if (playerResult is not null)
				playerResults[playerId] = playerResult;
		}
		return new OnUniversalTickResultDto(
			IsRoundFinished: false,
			// partial: only the keys named here are written, the rest of the state stays as it is
			PublicState: null,
			PrivateState: null,
			PlayerResults: playerResults.Count > 0 ? playerResults : null
		);
	}
}
