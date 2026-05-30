// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class King(PieceColor color, Position position, int moveCount = 0) : Piece(color, position, moveCount) {
    private static readonly BoardVector[] Offsets =
    [
        BoardVector.North,
        BoardVector.South,
        BoardVector.East,
        BoardVector.West,
        BoardVector.NorthEast,
        BoardVector.NorthWest,
        BoardVector.SouthEast,
        BoardVector.SouthWest,
    ];

    public override PieceType Type => PieceType.King;

    public override string Symbol => "K";

    public override IReadOnlyCollection<Position> GetAvailableMoves(Board board) {
        return MoveGeneration.GetSteppingMoves(this, board, Offsets);
    }

    public override Piece Clone() {
        return new King(Color, Position, MoveCount);
    }
}
