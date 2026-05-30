// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Pawn(PieceColor color, position_t position, int moveCount = 0) : Piece(color, position, moveCount) {
    public override PieceType get_type => PieceType.Pawn;

    public override string get_symbol => "P";

    public override IReadOnlyCollection<position_t> get_available_moves(Board board) {
        var result = new List<position_t>();
        var direction = get_color == PieceColor.White ? -1 : 1;

        var singleStep = get_position + new board_vector_t(direction, 0);
        if (singleStep.is_valid && board.IsEmpty(singleStep)) {
            result.Add(singleStep);

            var doubleStep = get_position + new board_vector_t(direction * 2, 0);
            if (get_move_count == 0 && doubleStep.is_valid && board.IsEmpty(doubleStep)) {
                result.Add(doubleStep);
            }
        }

        foreach (var attack_offset in new[] { new board_vector_t(direction, -1), new board_vector_t(direction, 1) }) {
            var attack_position = get_position + attack_offset;
            if (attack_position.is_valid && board.IsEnemy(attack_position, get_color)) {
                result.Add(attack_position);
            }
        }

        return result;
    }

    public override bool can_attack(position_t target, Board board) {
        var direction = get_color == PieceColor.White ? -1 : 1;
        return target == get_position + new board_vector_t(direction, -1)
            || target == get_position + new board_vector_t(direction, 1);
    }

    public override Piece make_clone() {
        return new Pawn(get_color, get_position, get_move_count);
    }
}
