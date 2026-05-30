// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Knight : Piece {
    private static readonly BoardVector[] Offsets =
    [
        new(-2, -1),
        new(-2, 1),
        new(-1, -2),
        new(-1, 2),
        new(1, -2),
        new(1, 2),
        new(2, -1),
        new(2, 1),
    ];

    public Knight(PieceColor color, Position position, int moveCount = 0)
        : base(color, position, moveCount) {
    }

    public override PieceType Type => PieceType.Knight;

    public override string Symbol => "N";

    public override IReadOnlyCollection<Position> GetAvailableMoves(Board board) {
        return MoveGeneration.GetSteppingMoves(this, board, Offsets);
    }

    public override Piece Clone() {
        return new Knight(Color, Position, MoveCount);
    }
}
