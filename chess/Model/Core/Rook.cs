// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Rook(PieceColor color, position_t position, int moveCount = 0) : Piece(color, position, moveCount) {
    public override PieceType get_type => PieceType.Rook;

    public override string get_symbol => "R";

    public override IReadOnlyCollection<position_t> get_available_moves(Board board) {
        return MoveGeneration.GetSlidingMoves(
            this,
            board,
            board_vector_t.north,
            board_vector_t.south,
            board_vector_t.east,
            board_vector_t.west);
    }

    public override Piece make_clone() => new Rook(get_color, get_position, get_move_count);
}
