// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public interface IAmPiece {
    PieceColor Color { get; }

    PieceType Type { get; }

    Position Position { get; }

    string Symbol { get; }

    IReadOnlyCollection<Position> GetAvailableMoves(Board board);

    bool CanAttack(Position target, Board board);
}
