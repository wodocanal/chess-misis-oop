// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Pawn(piece_color_t color, position_t position, int moveCount = 0) : piece_t(color, position, moveCount) {
    public override piece_type_t get_type => piece_type_t.PIECE_PAWN;

    public override string get_symbol => "P";

    public override IReadOnlyCollection<position_t> get_available_moves(board_t board) {
        var result = new List<position_t>();
        var direction = get_color == piece_color_t.PIECE_COLOR_WHITE ? -1 : 1;

        var singleStep = get_position + new board_vector_t(direction, 0);
        if (singleStep.is_valid && board.is_empty(singleStep)) {
            result.Add(singleStep);

            var doubleStep = get_position + new board_vector_t(direction * 2, 0);
            if (get_move_count == 0 && doubleStep.is_valid && board.is_empty(doubleStep)) {
                result.Add(doubleStep);
            }
        }

        foreach (var attack_offset in new[] { new board_vector_t(direction, -1), new board_vector_t(direction, 1) }) {
            var attack_position = get_position + attack_offset;
            if (attack_position.is_valid && board.is_enemy(attack_position, get_color)) {
                result.Add(attack_position);
            }
        }

        return result;
    }

    public override bool can_attack(position_t target, board_t board) {
        var direction = get_color == piece_color_t.PIECE_COLOR_WHITE ? -1 : 1;
        return target == get_position + new board_vector_t(direction, -1)
            || target == get_position + new board_vector_t(direction, 1);
    }

    public override piece_t make_clone() => new Pawn(get_color, get_position, get_move_count);
}
