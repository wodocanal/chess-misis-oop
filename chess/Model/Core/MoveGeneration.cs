namespace Model.Core;

internal static class MoveGeneration {
    public static IReadOnlyCollection<Position> GetSlidingMoves(Piece piece, Board board, params BoardVector[] directions) {
        var result = new List<Position>();

        foreach (var direction in directions) {
            var cursor = piece.Position + direction;
            while (cursor.IsValid) {
                if (board.IsEmpty(cursor)) {
                    result.Add(cursor);
                    cursor += direction;
                    continue;
                }

                if (board.IsEnemy(cursor, piece.Color)) {
                    result.Add(cursor);
                }

                break;
            }
        }

        return result;
    }

    public static IReadOnlyCollection<Position> GetSteppingMoves(Piece piece, Board board, params BoardVector[] offsets) {
        return [.. offsets
            .Select(offset => piece.Position + offset)
            .Where(position => piece.CanOccupy(board, position))];
    }
}
