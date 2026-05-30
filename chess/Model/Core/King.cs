// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class King(PieceColor color, position_t position, int moveCount = 0) : Piece(color, position, moveCount) {
    private static readonly board_vector_t[] offsets =
    [
        board_vector_t.north,
        board_vector_t.south,
        board_vector_t.east,
        board_vector_t.west,
        board_vector_t.north_east,
        board_vector_t.north_west,
        board_vector_t.south_east,
        board_vector_t.south_west,
    ];

    public override PieceType get_type => PieceType.King;

    public override string get_symbol => "K";

    public override IReadOnlyCollection<position_t> get_available_moves(Board board) {
        return MoveGeneration.GetSteppingMoves(this, board, offsets);
    }

    public override Piece make_clone() {
        return new King(get_color, get_position, get_move_count);
    }
}
