// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public sealed class move_snapshot_t {
    public piece_type_t piece_type { get; set; }

    public piece_color_t piece_color { get; set; }

    public int from_row { get; set; }

    public int from_column { get; set; }

    public int to_row { get; set; }

    public int to_column { get; set; }

    public string? captured_piece_type { get; set; }
}
