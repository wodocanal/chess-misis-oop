// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class GridMap<T>(int rows, int columns) {
    private readonly T?[,] _cells = new T?[rows, columns];

    public int Rows { get; } = rows;

    public int Columns { get; } = columns;

    public T? Get(position_t position) {
        return _cells[position.row, position.column];
    }

    public void Set(position_t position, T? value) {
        _cells[position.row, position.column] = value;
    }

    public IEnumerable<(position_t Position, T? Value)> Enumerate() {
        for (var row = 0; row < Rows; row += 1) {
            for (var column = 0; column < Columns; column += 1) {
                var position = new position_t(row, column);
                yield return (position, _cells[row, column]);
            }
        }
    }
}
