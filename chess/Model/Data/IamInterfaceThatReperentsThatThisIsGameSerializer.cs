// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

public interface IamInterfaceThatReperentsThatThisIsGameSerializer {
    serialization_format_t get_format { get; }

    string get_file_extension { get; }

    bool can_read(string file_path);

    void save(chess_game_t game, string file_path);

    chess_game_t load(string file_path);
}