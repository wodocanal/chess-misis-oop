// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public abstract class Piece(PieceColor color, position_t position, int moveCount = 0) : IamInterfaceThatReperentsThatThisIsPiece {
    public PieceColor get_color { get; } = color;

    public abstract PieceType get_type { get; }

    public position_t get_position { get; private set; } = position;

    public int get_move_count { get; private set; } = moveCount;

    public abstract string get_symbol { get; }

    public virtual bool can_attack(position_t target, Board board) {
        return get_available_moves(board).Contains(target);
    }

    public abstract IReadOnlyCollection<position_t> get_available_moves(Board board);

    public virtual void move_to(position_t target) {
        get_position = target;
        get_move_count += 1;
    }

    public abstract Piece make_clone();

    internal bool can_occupy_tokmachka(Board board, position_t position) {
        return position.is_valid && !board.IsFriendly(position, get_color);
    }
}
