namespace Model.Core;

public sealed class Bishop : Piece
{
	public Bishop(PieceColor color, Position position, int moveCount = 0)
		: base(color, position, moveCount)
	{
	}

	public override PieceType Type => PieceType.Bishop;

	public override string Symbol => "B";

	public override IReadOnlyCollection<Position> GetAvailableMoves(Board board)
	{
		return MoveGeneration.GetSlidingMoves(
			this,
			board,
			BoardVector.NorthEast,
			BoardVector.NorthWest,
			BoardVector.SouthEast,
			BoardVector.SouthWest);
	}

	public override Piece Clone()
	{
		return new Bishop(Color, Position, MoveCount);
	}
}
