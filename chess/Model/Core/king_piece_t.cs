// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class king_piece_t(piece_color_t color, position_t position, int moveCount = 0) : piece_t(color, position, moveCount) {
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

    public override piece_type_t get_type => piece_type_t.PIECE_TYPE_KING;

    public override string get_symbol => "K";

    public override IReadOnlyCollection<position_t> get_available_moves(board_t board) => MoveGenerator.generate_stepping_moves(this, board, offsets);

    public override piece_t make_clone() => new king_piece_t(get_color, get_position, get_move_count);
}
