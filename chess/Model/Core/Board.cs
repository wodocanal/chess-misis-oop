// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Board {
    private readonly GridMap<Piece> _cells = new(8, 8);

    public Piece? GetPiece(position_t position) => position.is_valid ? _cells.Get(position) : null;

    public Piece? GetPiece(int row, int column) => GetPiece(new position_t(row, column));

    public bool IsEmpty(position_t position) => GetPiece(position) is null;

    public bool IsEnemy(position_t position, PieceColor color) {
        var piece = GetPiece(position);
        return piece is not null && piece.get_color != color;
    }

    public bool IsFriendly(position_t position, PieceColor color) {
        var piece = GetPiece(position);
        return piece is not null && piece.get_color == color;
    }

    public void PlacePiece(Piece piece) => _cells.Set(piece.get_position, piece);

    public void SetPiece(position_t position, Piece? piece) => _cells.Set(position, piece);

    public void Clear(position_t position) => _cells.Set(position, null);

    public bool TryMove(position_t from, position_t to, out Piece? capturedPiece) {
        capturedPiece = null;
        var movingPiece = GetPiece(from);
        if (movingPiece is null) {
            return false;
        }

        capturedPiece = GetPiece(to);
        Clear(from);
        Clear(to);
        movingPiece.move_to(to);
        PlacePiece(movingPiece);
        return true;
    }

    public IReadOnlyCollection<Piece> GetPieces() {
        return [.. _cells.Enumerate()
            .Select(entry => entry.Value)
            .OfType<Piece>()];
    }

    public IReadOnlyCollection<TPiece> GetPieces<TPiece>() where TPiece : Piece => [.. GetPieces().OfType<TPiece>()];

    public Board Clone() {
        var clone = new Board();
        foreach (var piece in GetPieces()) {
            clone.PlacePiece(piece.make_clone());
        }

        return clone;
    }
}
