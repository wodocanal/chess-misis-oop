// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Pawn(PieceColor color, Position position, int moveCount = 0) : Piece(color, position, moveCount) {
    public override PieceType get_type => PieceType.Pawn;

    public override string get_symbol => "P";

    public override IReadOnlyCollection<Position> get_available_moves(Board board) {
        var result = new List<Position>();
        var direction = get_color == PieceColor.White ? -1 : 1;

        var singleStep = get_position + new BoardVector(direction, 0);
        if (singleStep.IsValid && board.IsEmpty(singleStep)) {
            result.Add(singleStep);

            var doubleStep = get_position + new BoardVector(direction * 2, 0);
            if (get_move_count == 0 && doubleStep.IsValid && board.IsEmpty(doubleStep)) {
                result.Add(doubleStep);
            }
        }

        foreach (var attackOffset in new[] { new BoardVector(direction, -1), new BoardVector(direction, 1) }) {
            var attackPosition = get_position + attackOffset;
            if (attackPosition.IsValid && board.IsEnemy(attackPosition, get_color)) {
                result.Add(attackPosition);
            }
        }

        return result;
    }

    public override bool can_attack(Position target, Board board) {
        var direction = get_color == PieceColor.White ? -1 : 1;
        return target == get_position + new BoardVector(direction, -1)
            || target == get_position + new BoardVector(direction, 1);
    }

    public override Piece make_clone() {
        return new Pawn(get_color, get_position, get_move_count);
    }
}
