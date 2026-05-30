// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

internal static class MoveGenerator {
    public static IReadOnlyCollection<position_t> generate_sliding_moves(piece_t piece, board_t board, params board_vector_t[] directions) {
        var ret = new List<position_t>();

        foreach (var direction in directions) {
            var cursor = piece.get_position + direction;

            while (cursor.is_valid) {
                if (board.is_empty(cursor)) {
                    ret.Add(cursor);
                    cursor += direction;
                    continue;
                }

                if (board.is_enemy(cursor, piece.get_color)) {
                    ret.Add(cursor);
                }

                break;
            }
        }

        return ret;
    }

    public static IReadOnlyCollection<position_t> generate_stepping_moves(piece_t piece, board_t board, params board_vector_t[] offsets) =>
        [.. offsets.Select(offset => piece.get_position + offset).Where(position => piece.can_occupy_tokmachka(board, position))];
}