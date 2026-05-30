// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class GridMap<T>(int rows, int columns) {
    private readonly T?[,] _cells = new T?[rows, columns];

    public int Rows { get; } = rows;

    public int Columns { get; } = columns;

    public T? Get(Position position) {
        return _cells[position.Row, position.Column];
    }

    public void Set(Position position, T? value) {
        _cells[position.Row, position.Column] = value;
    }

    public IEnumerable<(Position Position, T? Value)> Enumerate() {
        for (var row = 0; row < Rows; row++) {
            for (var column = 0; column < Columns; column++) {
                var position = new Position(row, column);
                yield return (position, _cells[row, column]);
            }
        }
    }
}
