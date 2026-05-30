// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public sealed class MoveSnapshot {
    public piece_type_t PieceType { get; set; }

    public piece_color_t PieceColor { get; set; }

    public int FromRow { get; set; }

    public int FromColumn { get; set; }

    public int ToRow { get; set; }

    public int ToColumn { get; set; }

    public string? CapturedPieceType { get; set; }
}
