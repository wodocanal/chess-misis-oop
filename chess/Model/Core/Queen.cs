// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Queen(PieceColor color, Position position, int moveCount = 0) : Piece(color, position, moveCount) {
    public override PieceType Type => PieceType.Queen;

    public override string Symbol => "Q";

    public override IReadOnlyCollection<Position> GetAvailableMoves(Board board) {
        return MoveGeneration.GetSlidingMoves(
            this,
            board,
            BoardVector.North,
            BoardVector.South,
            BoardVector.East,
            BoardVector.West,
            BoardVector.NorthEast,
            BoardVector.NorthWest,
            BoardVector.SouthEast,
            BoardVector.SouthWest);
    }

    public override Piece Clone() {
        return new Queen(Color, Position, MoveCount);
    }
}
