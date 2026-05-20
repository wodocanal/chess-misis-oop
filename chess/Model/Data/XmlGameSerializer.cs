using System.Xml.Serialization;

namespace Model.Data;

public sealed class XmlGameSerializer : GameSerializerBase
{
	public override SerializationFormat Format => SerializationFormat.Xml;

	public override string FileExtension => ".xml";

	protected override string SerializeSnapshot(GameSnapshot snapshot)
	{
		var serializer = new XmlSerializer(typeof(GameSnapshot));
		using var writer = new StringWriter();
		serializer.Serialize(writer, snapshot);
		return writer.ToString();
	}

	protected override GameSnapshot DeserializeSnapshot(string content)
	{
		var serializer = new XmlSerializer(typeof(GameSnapshot));
		using var reader = new StringReader(content);
		return serializer.Deserialize(reader) as GameSnapshot
			?? throw new InvalidOperationException("Unable to deserialize XML save file.");
	}
}
