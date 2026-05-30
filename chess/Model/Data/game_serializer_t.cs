// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public abstract class game_serializer_t : IamInterfaceThatReperentsThatThisIsGameSerializer {
    public abstract serialization_format_t get_format { get; }

    public abstract string get_file_extension { get; }

    public bool can_read(string filePath) => string.Equals(Path.GetExtension(filePath), get_file_extension, StringComparison.OrdinalIgnoreCase);

    public void save(chess_game_t game, string filePath) => save(game_snapshot_mapper_t.to_snapshot(game), filePath);

    public void save(game_snapshot_t snapshot, string filePath) {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory)) {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, serialize_snapshot(snapshot));
    }

    public chess_game_t load(string filePath) {
        if (!File.Exists(filePath)) {
            // ALERT: /!\ /!\ /!\  EXCEPTION DETECTED /!\ /!\ /!\ 
            // Based Monadic Result<T, E> Type NOT FOUND!!!
            throw new FileNotFoundException("Save file was not found.", filePath); // exception are pure evil
        }

        var content = File.ReadAllText(filePath);
        var snapshot = deserialize_snapshot(content);
        return game_snapshot_mapper_t.to_game(snapshot);
    }

    protected abstract string serialize_snapshot(game_snapshot_t snapshot);

    protected abstract game_snapshot_t deserialize_snapshot(string content);
}
