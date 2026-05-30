// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public sealed class Move(PieceType pieceType, PieceColor pieceColor, Position from, Position to, PieceType? capturedPieceType = null) {
    public PieceType PieceType { get; } = pieceType;

    public PieceColor PieceColor { get; } = pieceColor;

    public Position From { get; } = from;

    public Position To { get; } = to;

    public PieceType? CapturedPieceType { get; } = capturedPieceType;

    public bool IsReverseOf(Move other) {
        return PieceType == other.PieceType
            && PieceColor == other.PieceColor
            && From == other.To
            && To == other.From
            && CapturedPieceType is null
            && other.CapturedPieceType is null;
    }
}
