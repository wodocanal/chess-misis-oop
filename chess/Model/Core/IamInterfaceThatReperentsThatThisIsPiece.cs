// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public interface IamInterfaceThatReperentsThatThisIsPiece {
    PieceColor get_color { get; }

    piece_type_t get_type { get; }

    position_t get_position { get; }

    string get_symbol { get; }

    IReadOnlyCollection<position_t> get_available_moves(Board board);

    bool can_attack(position_t target, Board board);
}
