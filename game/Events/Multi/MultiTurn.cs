namespace Flappybet;

public partial class Game : IGame
{
	public static TakenSeat GetNextSeat(Turn? currentTurn, List<PlayerData> players)
	{
		var prevSeat = currentTurn ?? new Turn(SeatId: -1);
		var seats = players.Select(p => p.Seat).OrderBy(s => s.SeatId).ToList();
		var nextSeat = seats.FirstOrDefault(s => s.SeatId > prevSeat.SeatId) ?? seats.First();
		return nextSeat;
	}

	public static OnMultiTurnResDto OnMultiTurn(OnMultiTurnDto input)
	{
		throw new NotImplementedException();
	}
}
