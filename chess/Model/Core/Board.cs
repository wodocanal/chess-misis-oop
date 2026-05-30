// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Board {
    private readonly GridMap<Piece> _cells = new(8, 8);

    public Piece? GetPiece(Position position) {
        return position.IsValid ? _cells.Get(position) : null;
    }

    public Piece? GetPiece(int row, int column) {
        return GetPiece(new Position(row, column));
    }

    public bool IsEmpty(Position position) {
        return GetPiece(position) is null;
    }

    public bool IsEnemy(Position position, PieceColor color) {
        var piece = GetPiece(position);
        return piece is not null && piece.get_color != color;
    }

    public bool IsFriendly(Position position, PieceColor color) {
        var piece = GetPiece(position);
        return piece is not null && piece.get_color == color;
    }

    public void PlacePiece(Piece piece) {
        _cells.Set(piece.get_position, piece);
    }

    public void SetPiece(Position position, Piece? piece) {
        _cells.Set(position, piece);
    }

    public void Clear(Position position) {
        _cells.Set(position, null);
    }

    public bool TryMove(Position from, Position to, out Piece? capturedPiece) {
        capturedPiece = null;
        var movingPiece = GetPiece(from);
        if (movingPiece is null) {
            return false;
        }

        capturedPiece = GetPiece(to);
        Clear(from);
        Clear(to);
        movingPiece.MoveTo(to);
        PlacePiece(movingPiece);
        return true;
    }

    public IReadOnlyCollection<Piece> GetPieces() {
        return [.. _cells.Enumerate()
            .Select(entry => entry.Value)
            .OfType<Piece>()];
    }

    public IReadOnlyCollection<TPiece> GetPieces<TPiece>() where TPiece : Piece {
        return [.. GetPieces().OfType<TPiece>()];
    }

    public Board Clone() {
        var clone = new Board();
        foreach (var piece in GetPieces()) {
            clone.PlacePiece(piece.Clone());
        }

        return clone;
    }
}
