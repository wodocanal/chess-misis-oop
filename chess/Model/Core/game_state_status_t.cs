// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public enum game_state_status_t {
    GAME_STATUS_IN_PROGRESS,
    GAME_STATUS_CHECK,
    GAME_STATUS_IN_CHECKMATE,
    GAME_STATUS_STALEMATE,
}
