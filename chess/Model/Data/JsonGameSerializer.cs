// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Model.Data;

public sealed class JsonGameSerializer : GameSerializerBase {
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
        },
    };

    public override SerializationFormat Format => SerializationFormat.Json;

    public override string FileExtension => ".json";

    protected override string SerializeSnapshot(GameSnapshot snapshot) => JsonSerializer.Serialize(snapshot, SerializerOptions);

    protected override GameSnapshot DeserializeSnapshot(string content) {
        return JsonSerializer.Deserialize<GameSnapshot>(content, SerializerOptions)
            ?? throw new InvalidOperationException("Unable to deserialize JSON save file.");
    }
}
