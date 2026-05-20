namespace Model.Core;

public readonly record struct Position(int Row, int Column) {
    public bool IsValid => Row is >= 0 and < 8 && Column is >= 0 and < 8;

    public static Position operator +(Position position, BoardVector vector) {
        return new Position(position.Row + vector.RowOffset, position.Column + vector.ColumnOffset);
    }

    public override string ToString() {
        return $"{(char)('A' + Column)}{8 - Row}";
    }
}
