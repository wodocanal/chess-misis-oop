// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Knight(piece_color_t color, position_t position, int moveCount = 0) : Piece(color, position, moveCount) {
    private static readonly board_vector_t[] offsets =
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

    public override piece_type_t get_type => piece_type_t.PIECE_KNIGHT;

    public override string get_symbol => "N";

    public override IReadOnlyCollection<position_t> get_available_moves(Board board) => MoveGeneration.GetSteppingMoves(this, board, offsets);

    public override Piece make_clone() => new Knight(get_color, get_position, get_move_count);
}
