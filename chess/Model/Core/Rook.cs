// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Rook(PieceColor color, Position position, int moveCount = 0) : Piece(color, position, moveCount) {
    public override PieceType get_type => PieceType.Rook;

    public override string get_symbol => "R";

    public override IReadOnlyCollection<Position> get_available_moves(Board board) {
        return MoveGeneration.GetSlidingMoves(
            this,
            board,
            BoardVector.North,
            BoardVector.South,
            BoardVector.East,
            BoardVector.West);
    }

    public override Piece Clone() {
        return new Rook(get_color, get_position, MoveCount);
    }
}
