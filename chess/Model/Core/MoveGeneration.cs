// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

internal static class MoveGeneration {
    public static IReadOnlyCollection<Position> GetSlidingMoves(Piece piece, Board board, params BoardVector[] directions) {
        var result = new List<Position>();

        foreach (var direction in directions) {
            var cursor = piece.get_position + direction;
            while (cursor.IsValid) {
                if (board.IsEmpty(cursor)) {
                    result.Add(cursor);
                    cursor += direction;
                    continue;
                }

                if (board.IsEnemy(cursor, piece.get_color)) {
                    result.Add(cursor);
                }

                break;
            }
        }

        return result;
    }

    public static IReadOnlyCollection<Position> GetSteppingMoves(Piece piece, Board board, params BoardVector[] offsets) {
        return [.. offsets
            .Select(offset => piece.get_position + offset)
            .Where(position => piece.CanOccupy(board, position))];
    }
}
