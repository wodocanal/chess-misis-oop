using Model.Core;

namespace Model.Data;

public sealed class GameSnapshot {
    public int SchemaVersion { get; set; } = 1;

    public PieceColor CurrentTurn { get; set; }

    public GameStateStatus Status { get; set; }

    public List<PieceSnapshot> Pieces { get; set; } = [];

    public List<MoveSnapshot> Moves { get; set; } = [];
}
