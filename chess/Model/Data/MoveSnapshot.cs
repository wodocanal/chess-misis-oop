using Model.Core;

namespace Model.Data;

public sealed class MoveSnapshot {
    public PieceType PieceType { get; set; }

    public PieceColor PieceColor { get; set; }

    public int FromRow { get; set; }

    public int FromColumn { get; set; }

    public int ToRow { get; set; }

    public int ToColumn { get; set; }

    public string? CapturedPieceType { get; set; }
}
