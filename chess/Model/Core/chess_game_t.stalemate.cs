// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public partial class chess_game_t {

    private bool is_stalemate(piece_color_t color) {
        if (is_in_check(color)) {
            return false;
        }

        if (!has_any_legal_move(color)) {
            return true;
        }

        return is_six_reversible_half_moves_reached();
    }

    private bool is_six_reversible_half_moves_reached() {
        if (this.move_history.Count < 6) {
            return false;
        }

        var lastMoves = this.move_history.TakeLast(6).ToArray();
        for (var index = 1; index < lastMoves.Length; index += 2) {
            if (!lastMoves[index].is_reverse_of(lastMoves[index - 1])) {
                return false;
            }
        }

        return true;
    }

}
