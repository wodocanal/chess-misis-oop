// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public abstract class Piece(PieceColor color, Position position, int moveCount = 0) : IAmPiece {
    public PieceColor Color { get; } = color;

    public abstract PieceType Type { get; }

    public Position Position { get; private set; } = position;

    public int MoveCount { get; private set; } = moveCount;

    public abstract string Symbol { get; }

    public virtual bool CanAttack(Position target, Board board) {
        return GetAvailableMoves(board).Contains(target);
    }

    public abstract IReadOnlyCollection<Position> GetAvailableMoves(Board board);

    public virtual void MoveTo(Position target) {
        Position = target;
        MoveCount += 1;
    }

    public abstract Piece Clone();

    internal bool CanOccupy(Board board, Position position) {
        return position.IsValid && !board.IsFriendly(position, Color);
    }
}
