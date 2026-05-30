// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Move(piece_type_t pieceType, piece_color_t pieceColor, position_t from, position_t to, piece_type_t? capturedPieceType = null) {
    public piece_type_t PieceType { get; } = pieceType;

    public piece_color_t PieceColor { get; } = pieceColor;

    public position_t From { get; } = from;

    public position_t To { get; } = to;

    public piece_type_t? CapturedPieceType { get; } = capturedPieceType;

    public bool IsReverseOf(Move other) {
        return PieceType == other.PieceType
            && PieceColor == other.PieceColor
            && From == other.To
            && To == other.From
            && CapturedPieceType is null
            && other.CapturedPieceType is null;
    }
}
