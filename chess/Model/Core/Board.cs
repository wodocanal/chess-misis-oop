// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Board {
    private readonly GridMap<Piece> cells = new(8, 8);

    public Piece? piece_get(position_t position) => position.is_valid ? this.cells.get(position) : null;

    public Piece? piece_get(int row, int column) => this.piece_get(new position_t(row, column));

    public bool is_empty(position_t position) => this.piece_get(position) is null;

    public bool is_enemy(position_t position, piece_color_t color) {
        var piece = this.piece_get(position);
        return piece is not null && piece.get_color != color;
    }

    public bool is_friendly(position_t position, piece_color_t color) {
        var piece = this.piece_get(position);
        return piece is not null && piece.get_color == color;
    }

    public void piece_place(Piece piece) => this.cells.put(piece.get_position, piece);

    public void piece_set(position_t position, Piece? piece) => this.cells.put(position, piece);

    public void piece_remove(position_t position) => this.cells.put(position, null);

    public bool try_move(position_t from, position_t to, out Piece? captured_piece) {
        captured_piece = null;
        var moving_piece = this.piece_get(from);
        if (moving_piece is null) { return false; }

        captured_piece = this.piece_get(to);
        this.piece_remove(from);
        this.piece_remove(to);
        moving_piece.move_to(to);
        this.piece_place(moving_piece);

        return true;
    }

    public IReadOnlyCollection<Piece> pieces_getall() => [.. this.cells.to_enumerable().Select(entry => entry.Value).OfType<Piece>()];

    public IReadOnlyCollection<T> pieces_getall<T>() where T : Piece => [.. this.pieces_getall().OfType<T>()];

    public Board make_clone() {
        var ret = new Board();

        foreach (var piece in this.pieces_getall()) {
            ret.piece_place(piece.make_clone());
        }

        return ret;
    }
}
