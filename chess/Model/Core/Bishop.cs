// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Bishop(PieceColor color, Position position, int moveCount = 0) : Piece(color, position, moveCount) {
    public override PieceType Type => PieceType.Bishop;

    public override string Symbol => "B";

    public override IReadOnlyCollection<Position> GetAvailableMoves(Board board) {
        return MoveGeneration.GetSlidingMoves(
            this,
            board,
            BoardVector.NorthEast,
            BoardVector.NorthWest,
            BoardVector.SouthEast,
            BoardVector.SouthWest);
    }

    public override Piece Clone() {
        return new Bishop(Color, Position, MoveCount);
    }
}
