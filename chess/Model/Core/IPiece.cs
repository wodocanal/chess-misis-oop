namespace Model.Core;

public interface IPiece
{
	PieceColor Color { get; }

	PieceType Type { get; }

	Position Position { get; }

	string Symbol { get; }

	IReadOnlyCollection<Position> GetAvailableMoves(Board board);

	bool CanAttack(Position target, Board board);
}
