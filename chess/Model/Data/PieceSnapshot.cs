using Model.Core;

namespace Model.Data;

public sealed class PieceSnapshot {
    public string Type { get; set; } = string.Empty;

    public PieceColor Color { get; set; }

    public int Row { get; set; }

    public int Column { get; set; }

    public int MoveCount { get; set; }
}
