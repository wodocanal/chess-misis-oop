namespace Model.Core;

public partial class ChessGame
{
	private bool IsStalemate(PieceColor color)
	{
		if (IsInCheck(color))
		{
			return false;
		}

		if (!HasAnyLegalMove(color))
		{
			return true;
		}

		return IsSixReversibleHalfMovesReached();
	}

	private bool IsSixReversibleHalfMovesReached()
	{
		if (_moveHistory.Count < 6)
		{
			return false;
		}

		var lastMoves = _moveHistory.TakeLast(6).ToArray();
		for (var index = 1; index < lastMoves.Length; index += 2)
		{
			if (!lastMoves[index].IsReverseOf(lastMoves[index - 1]))
			{
				return false;
			}
		}

		return true;
	}
}
