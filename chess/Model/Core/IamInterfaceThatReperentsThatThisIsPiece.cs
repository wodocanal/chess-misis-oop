// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public interface IamInterfaceThatReperentsThatThisIsPiece {
    PieceColor get_color { get; }

    PieceType get_type { get; }

    Position get_position { get; }

    string get_symbol { get; }

    IReadOnlyCollection<Position> get_available_moves(Board board);

    bool can_attack(Position target, Board board);
}
