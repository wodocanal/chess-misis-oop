// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public partial class ChessGame {
    private bool IsStalemate(PieceColor color) {
        if (IsInCheck(color)) {
            return false;
        }

        if (!HasAnyLegalMove(color)) {
            return true;
        }

        return IsSixReversibleHalfMovesReached();
    }

    private bool IsSixReversibleHalfMovesReached() {
        if (_moveHistory.Count < 6) {
            return false;
        }

        var lastMoves = _moveHistory.TakeLast(6).ToArray();
        for (var index = 1; index < lastMoves.Length; index += 2) {
            if (!lastMoves[index].IsReverseOf(lastMoves[index - 1])) {
                return false;
            }
        }

        return true;
    }
}
