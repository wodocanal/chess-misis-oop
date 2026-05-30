// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using System.Xml.Serialization;

namespace Model.Data;

public sealed class XmlGameSerializer : GameSerializerBase {
    public override SerializationFormat Format => SerializationFormat.Xml;

    public override string FileExtension => ".xml";

    protected override string SerializeSnapshot(GameSnapshot snapshot) {
        var serializer = new XmlSerializer(typeof(GameSnapshot));
        using var writer = new StringWriter();
        serializer.Serialize(writer, snapshot);
        return writer.ToString();
    }

    protected override GameSnapshot DeserializeSnapshot(string content) {
        var serializer = new XmlSerializer(typeof(GameSnapshot));
        using var reader = new StringReader(content);
        return serializer.Deserialize(reader) as GameSnapshot
            ?? throw new InvalidOperationException("Unable to deserialize XML save file.");
    }
}
