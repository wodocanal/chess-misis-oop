// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public interface GameSerializer {
    SerializationFormat Format { get; }

    string FileExtension { get; }

    bool CanRead(string filePath);

    void Save(chess_game_t game, string filePath);

    chess_game_t Load(string filePath);
}
