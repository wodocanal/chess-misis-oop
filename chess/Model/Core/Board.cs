// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Board {
    private readonly GridMap<Piece> cells = new(8, 8);

    public Piece? get_piece(position_t position) => position.is_valid ? this.cells.get(position) : null;

    public Piece? get_piece(int row, int column) => this.get_piece(new position_t(row, column));

    public bool is_empty(position_t position) => this.get_piece(position) is null;

    public bool is_enemy(position_t position, piece_color_t color) {
        var piece = this.get_piece(position);
        return piece is not null && piece.get_color != color;
    }

    public bool is_friendly(position_t position, piece_color_t color) {
        var piece = this.get_piece(position);
        return piece is not null && piece.get_color == color;
    }

    public void place_piece(Piece piece) => this.cells.put(piece.get_position, piece);

    public void set_piece(position_t position, Piece? piece) => this.cells.put(position, piece);

    public void clear(position_t position) => this.cells.put(position, null);

    public bool try_move(position_t from, position_t to, out Piece? captured_piece) {
        captured_piece = null;
        var moving_piece = this.get_piece(from);
        if (moving_piece is null) { return false; }

        captured_piece = this.get_piece(to);
        this.clear(from);
        this.clear(to);
        moving_piece.move_to(to);
        this.place_piece(moving_piece);

        return true;
    }

    public IReadOnlyCollection<Piece> get_pieces() => [.. this.cells.to_enumerable().Select(entry => entry.Value).OfType<Piece>()];

    public IReadOnlyCollection<T> get_pieces<T>() where T : Piece => [.. this.get_pieces().OfType<T>()];

    public Board make_clone() {
        var ret = new Board();

        foreach (var piece in this.get_pieces()) {
            ret.place_piece(piece.make_clone());
        }

        return ret;
    }
}
