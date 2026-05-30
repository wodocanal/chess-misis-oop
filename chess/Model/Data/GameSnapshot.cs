// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public sealed class GameSnapshot {
    public int SchemaVersion { get; set; } = 1;

    public piece_color_t CurrentTurn { get; set; }

    public GameStateStatus Status { get; set; }

    public List<PieceSnapshot> Pieces { get; set; } = [];

    public List<MoveSnapshot> Moves { get; set; } = [];
}
