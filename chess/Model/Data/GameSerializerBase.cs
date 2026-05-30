// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public abstract class GameSerializerBase : GameSerializer {
    public abstract SerializationFormat Format { get; }

    public abstract string FileExtension { get; }

    public bool CanRead(string filePath) => string.Equals(Path.GetExtension(filePath), FileExtension, StringComparison.OrdinalIgnoreCase);

    public void Save(chess_game_t game, string filePath) => Save(GameSnapshotMapper.ToSnapshot(game), filePath);

    public void Save(GameSnapshot snapshot, string filePath) {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory)) {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, SerializeSnapshot(snapshot));
    }

    public chess_game_t Load(string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException("Save file was not found.", filePath);
        }

        var content = File.ReadAllText(filePath);
        var snapshot = DeserializeSnapshot(content);
        return GameSnapshotMapper.ToGame(snapshot);
    }

    protected abstract string SerializeSnapshot(GameSnapshot snapshot);

    protected abstract GameSnapshot DeserializeSnapshot(string content);
}
