// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Model.Data;

public sealed class json_game_serializer_t : game_serializer_t {
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
        },
    };

    public override serialization_format_t get_format => serialization_format_t.SERIALIZATION_FORMAT_JSON;

    public override string get_file_extension => ".json";

    protected override string serialize_snapshot(game_snapshot_t snapshot) => JsonSerializer.Serialize(snapshot, SerializerOptions);

    protected override game_snapshot_t deserialize_snapshot(string content) {
        return JsonSerializer.Deserialize<game_snapshot_t>(content, SerializerOptions)
            ?? throw new InvalidOperationException("Unable to deserialize JSON save file.");
    }
}
