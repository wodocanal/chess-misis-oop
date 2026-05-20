using System.Text.Json;
using System.Text.Json.Serialization;

namespace Model.Data;

public sealed class JsonGameSerializer : GameSerializerBase
{
	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		WriteIndented = true,
		Converters =
		{
			new JsonStringEnumConverter(),
		},
	};

	public override SerializationFormat Format => SerializationFormat.Json;

	public override string FileExtension => ".json";

	protected override string SerializeSnapshot(GameSnapshot snapshot)
	{
		return JsonSerializer.Serialize(snapshot, SerializerOptions);
	}

	protected override GameSnapshot DeserializeSnapshot(string content)
	{
		return JsonSerializer.Deserialize<GameSnapshot>(content, SerializerOptions)
			?? throw new InvalidOperationException("Unable to deserialize JSON save file.");
	}
}
