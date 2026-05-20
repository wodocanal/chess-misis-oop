using Model.Core;

namespace Model.Data;

public abstract class GameSerializerBase : IGameSerializer
{
	public abstract SerializationFormat Format { get; }

	public abstract string FileExtension { get; }

	public bool CanRead(string filePath)
	{
		return string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase);
	}

	public void Save(ChessGame game, string filePath)
	{
		Save(GameSnapshotMapper.ToSnapshot(game), filePath);
	}

	public void Save(GameSnapshot snapshot, string filePath)
	{
		var directory = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(filePath, SerializeSnapshot(snapshot));
	}

	public ChessGame Load(string filePath)
	{
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException("Save file was not found.", filePath);
		}

		var content = File.ReadAllText(filePath);
		var snapshot = DeserializeSnapshot(content);
		return GameSnapshotMapper.ToGame(snapshot);
	}

	protected abstract string SerializeSnapshot(GameSnapshot snapshot);

	protected abstract GameSnapshot DeserializeSnapshot(string content);
}
