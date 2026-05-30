// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public sealed class game_snapshot_t {
    public int get_schema_version { get; set; } = 1;

    public piece_color_t get_current_turn { get; set; }

    public game_state_status_t get_status { get; set; }

    public List<piece_snapshot_t> get_pieces { get; set; } = [];

    public List<move_snapshot_t> get_moves { get; set; } = [];
}