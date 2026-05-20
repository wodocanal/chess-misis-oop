namespace Model.Core;

public sealed class Rook : Piece
{
	public Rook(PieceColor color, Position position, int moveCount = 0)
		: base(color, position, moveCount)
	{
	}

	public override PieceType Type => PieceType.Rook;

	public override string Symbol => "R";

	public override IReadOnlyCollection<Position> GetAvailableMoves(Board board)
	{
		return MoveGeneration.GetSlidingMoves(
			this,
			board,
			BoardVector.North,
			BoardVector.South,
			BoardVector.East,
			BoardVector.West);
	}

	public override Piece Clone()
	{
		return new Rook(Color, Position, MoveCount);
	}
}
