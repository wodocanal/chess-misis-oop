// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public enum move_execution_result_t {
    MOVE_EXECUTION_RESULT_SUCCESS,
    MOVE_EXECUTION_RESULT_CANCELLED_SELECTION,
    MOVE_EXECUTION_RESULT_INVALID_SOURCE,
    MOVE_EXECUTION_RESULT_INVALID_TARGET,
    MOVE_EXECUTION_RESULT_WRONG_TURN,
    MOVE_EXECUTION_RESULT_GAME_FINISHED,
}