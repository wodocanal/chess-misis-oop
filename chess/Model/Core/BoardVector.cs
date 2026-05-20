namespace Model.Core;

public readonly record struct BoardVector(int RowOffset, int ColumnOffset) {
    public static readonly BoardVector North = new(-1, 0);
    public static readonly BoardVector South = new(1, 0);
    public static readonly BoardVector East = new(0, 1);
    public static readonly BoardVector West = new(0, -1);
    public static readonly BoardVector NorthEast = new(-1, 1);
    public static readonly BoardVector NorthWest = new(-1, -1);
    public static readonly BoardVector SouthEast = new(1, 1);
    public static readonly BoardVector SouthWest = new(1, -1);
}
