// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public abstract class GameSerializerBase : IamInterfaceThatReperentsThatThisIsGameSerializer {
    public abstract serialization_format_t get_format { get; }

    public abstract string get_file_extension { get; }

    public bool can_read(string filePath) => string.Equals(Path.GetExtension(filePath), get_file_extension, StringComparison.OrdinalIgnoreCase);

    public void save(chess_game_t game, string filePath) => Save(GameSnapshotMapper.ToSnapshot(game), filePath);

    public void Save(game_snapshot_t snapshot, string filePath) {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory)) {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, SerializeSnapshot(snapshot));
    }

    public chess_game_t load(string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("Save file was not found.", filePath);
        }

        var content = File.ReadAllText(filePath);
        var snapshot = DeserializeSnapshot(content);
        return GameSnapshotMapper.ToGame(snapshot);
    }

    protected abstract string SerializeSnapshot(game_snapshot_t snapshot);

    protected abstract game_snapshot_t DeserializeSnapshot(string content);
}
