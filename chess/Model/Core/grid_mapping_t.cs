// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class grid_mapping_t<T>(int rows, int columns) {
    private readonly T?[,] cells = new T?[rows, columns];

    public int get_rows { get; } = rows;

    public int get_columns { get; } = columns;

    public T? get(position_t position) => this.cells[position.row, position.column];

    public void put(position_t position, T? value) => this.cells[position.row, position.column] = value;

    public IEnumerable<(position_t Position, T? Value)> to_enumerable() {
        for (var row = 0; row < get_rows; row += 1) {
            for (var column = 0; column < get_columns; column += 1) {
                var position = new position_t(row, column);
                yield return (position, this.cells[row, column]);
            }
        }
    }
}
