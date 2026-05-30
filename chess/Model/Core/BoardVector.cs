// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public readonly record struct BoardVector(int RowOffset, int ColumnOffset) {
    public static readonly BoardVector North = new(-1, 0);
    public static readonly BoardVector South = new(1, 0);
    public static readonly BoardVector East = new(0, 1);
    public static readonly BoardVector West = new(0, -1);
    public static readonly BoardVector NorthEast = new(-1, 1);
    public static readonly BoardVector NorthWest = new(-1, -1);
    public static readonly BoardVector SouthEast = new(1, 1);
    public static readonly BoardVector SouthWest = new(1, -1);
}
