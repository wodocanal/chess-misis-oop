namespace Model.Core;

public sealed class Queen : Piece {
    public Queen(PieceColor color, Position position, int moveCount = 0)
        : base(color, position, moveCount) {
    }

    public override PieceType Type => PieceType.Queen;

    public override string Symbol => "Q";

    public override IReadOnlyCollection<Position> GetAvailableMoves(Board board) {
        return MoveGeneration.GetSlidingMoves(
            this,
            board,
            BoardVector.North,
            BoardVector.South,
            BoardVector.East,
            BoardVector.West,
            BoardVector.NorthEast,
            BoardVector.NorthWest,
            BoardVector.SouthEast,
            BoardVector.SouthWest);
    }

    public override Piece Clone() {
        return new Queen(Color, Position, MoveCount);
    }
}
