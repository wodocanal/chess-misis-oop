// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public sealed class PieceSnapshot {
    public string Type { get; set; } = string.Empty;

    public piece_color_t Color { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    public int MoveCount { get; set; }
}
