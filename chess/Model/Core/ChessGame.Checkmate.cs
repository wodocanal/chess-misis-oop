namespace Model.Core;

public partial class ChessGame
{
	private bool IsCheckmate(PieceColor color)
	{
		return IsInCheck(color) && !HasAnyLegalMove(color);
	}
}
