// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public sealed class piece_snapshot_t {
    public string type { get; set; } = string.Empty;

    public piece_color_t color { get; set; }

    public int row { get; set; }

    public int column { get; set; }

    public int move_count { get; set; }
}