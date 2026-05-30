// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public readonly record struct board_vector_t(int row_offset, int column_offset) {
    public static readonly board_vector_t
        north = new(-1, 0),
        south = new(+1, 0),
        east = new(0, +1),
        west = new(0, -1),
        north_east = new(-1, +1),
        north_west = new(-1, -1),
        south_east = new(+1, +1),
        south_west = new(+1, -1)
    ;
}