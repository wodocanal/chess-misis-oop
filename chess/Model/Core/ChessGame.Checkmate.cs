// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public partial class ChessGame {
    private bool IsCheckmate(PieceColor color) {
        return IsInCheck(color) && !HasAnyLegalMove(color);
    }
}
