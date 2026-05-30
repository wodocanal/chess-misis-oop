// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class piece_move_t(piece_type_t piece_type, piece_color_t piece_color, position_t from, position_t to, piece_type_t? captured_piece_type = null) {
    public piece_type_t get_piece_type { get; } = piece_type;

    public piece_color_t get_piece_color { get; } = piece_color;

    public position_t get_position_from { get; } = from;

    public position_t get_position_to { get; } = to;

    public piece_type_t? get_captured_piece_type { get; } = captured_piece_type;

    public bool is_reverse_of(piece_move_t other) {
        return get_piece_type == other.get_piece_type
            && get_piece_color == other.get_piece_color
            && get_position_from == other.get_position_to
            && get_position_to == other.get_position_from
            && get_captured_piece_type is null
            && other.get_captured_piece_type is null;
    }
}
