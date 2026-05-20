namespace Model.Core;

public abstract class Piece : IPiece {
    protected Piece(PieceColor color, Position position, int moveCount = 0) {
        Color = color;
        Position = position;
        MoveCount = moveCount;
    }

    public PieceColor Color { get; }

    public abstract PieceType Type { get; }

    public Position Position { get; private set; }

    public int MoveCount { get; private set; }

    public abstract string Symbol { get; }

    public virtual bool CanAttack(Position target, Board board) {
        return GetAvailableMoves(board).Contains(target);
    }

    public abstract IReadOnlyCollection<Position> GetAvailableMoves(Board board);

    public virtual void MoveTo(Position target) {
        Position = target;
        MoveCount++;
    }

    public abstract Piece Clone();

    internal bool CanOccupy(Board board, Position position) {
        return position.IsValid && !board.IsFriendly(position, Color);
    }
}
