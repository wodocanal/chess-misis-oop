// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Pawn : Piece {
    public Pawn(PieceColor color, Position position, int moveCount = 0)
        : base(color, position, moveCount) {
    }

    public override PieceType Type => PieceType.Pawn;

    public override string Symbol => "P";

    public override IReadOnlyCollection<Position> GetAvailableMoves(Board board) {
        var result = new List<Position>();
        var direction = Color == PieceColor.White ? -1 : 1;

        var singleStep = Position + new BoardVector(direction, 0);
        if (singleStep.IsValid && board.IsEmpty(singleStep)) {
            result.Add(singleStep);

            var doubleStep = Position + new BoardVector(direction * 2, 0);
            if (MoveCount == 0 && doubleStep.IsValid && board.IsEmpty(doubleStep)) {
                result.Add(doubleStep);
            }
        }

        foreach (var attackOffset in new[] { new BoardVector(direction, -1), new BoardVector(direction, 1) }) {
            var attackPosition = Position + attackOffset;
            if (attackPosition.IsValid && board.IsEnemy(attackPosition, Color)) {
                result.Add(attackPosition);
            }
        }

        return result;
    }

    public override bool CanAttack(Position target, Board board) {
        var direction = Color == PieceColor.White ? -1 : 1;
        return target == Position + new BoardVector(direction, -1)
            || target == Position + new BoardVector(direction, 1);
    }

    public override Piece Clone() {
        return new Pawn(Color, Position, MoveCount);
    }
}
