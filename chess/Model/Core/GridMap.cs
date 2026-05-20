namespace Model.Core;

public sealed class GridMap<T>
{
	private readonly T?[,] _cells;

	public GridMap(int rows, int columns)
	{
		Rows = rows;
		Columns = columns;
		_cells = new T?[rows, columns];
	}

	public int Rows { get; }

	public int Columns { get; }

	public T? Get(Position position)
	{
		return _cells[position.Row, position.Column];
	}

	public void Set(Position position, T? value)
	{
		_cells[position.Row, position.Column] = value;
	}

	public IEnumerable<(Position Position, T? Value)> Enumerate()
	{
		for (var row = 0; row < Rows; row++)
		{
			for (var column = 0; column < Columns; column++)
			{
				var position = new Position(row, column);
				yield return (position, _cells[row, column]);
			}
		}
	}
}
