// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Knight(PieceColor color, Position position, int moveCount = 0) : Piece(color, position, moveCount) {
    private static readonly BoardVector[] offsets =
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

    public override PieceType get_type => PieceType.Knight;

    public override string get_symbol => "N";

    public override IReadOnlyCollection<Position> get_available_moves(Board board) {
        return MoveGeneration.GetSteppingMoves(this, board, offsets);
    }

    public override Piece make_clone() {
        return new Knight(get_color, get_position, get_move_count);
    }
}
