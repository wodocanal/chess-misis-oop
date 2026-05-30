// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using System.Xml.Serialization;

namespace Model.Data;

public sealed class xml_game_serializer_t : game_serializer_t {
    public override serialization_format_t get_format => serialization_format_t.SERIALIZATION_FORMAT_XML;

    public override string get_file_extension => ".xml";

    protected override string serialize_snapshot(game_snapshot_t snapshot) {
        var serializer = new XmlSerializer(typeof(game_snapshot_t));
        using var writer = new StringWriter();
        serializer.Serialize(writer, snapshot);
        return writer.ToString();
    }

    protected override game_snapshot_t deserialize_snapshot(string content) {
        var serializer = new XmlSerializer(typeof(game_snapshot_t));
        using var reader = new StringReader(content);
        return serializer.Deserialize(reader) as game_snapshot_t
            ?? throw new InvalidOperationException("Unable to deserialize XML save file.");
    }
}
