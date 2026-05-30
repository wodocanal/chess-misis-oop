// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

internal static class MoveGeneration {
    public static IReadOnlyCollection<position_t> GetSlidingMoves(Piece piece, Board board, params board_vector_t[] directions) {
        var result = new List<position_t>();

        foreach (var direction in directions) {
            var cursor = piece.get_position + direction;
            while (cursor.is_valid) {
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

    public static IReadOnlyCollection<position_t> GetSteppingMoves(Piece piece, Board board, params board_vector_t[] offsets) {
        return [.. offsets
            .Select(offset => piece.get_position + offset)
            .Where(position => piece.can_occupy_tokmachka(board, position))];
    }
}
