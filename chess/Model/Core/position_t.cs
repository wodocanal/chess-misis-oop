// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public readonly record struct position_t(int row, int column) {
    public bool is_valid => row is >= 0 and < 8 && column is >= 0 and < 8;

    public static position_t operator +(position_t position, board_vector_t vector) {
        return new position_t(position.row + vector.row_offset, position.column + vector.column_offset);
    }

    public override string ToString() {
        return $"{(char)('A' + column)}{8 - row}";
    }
}
