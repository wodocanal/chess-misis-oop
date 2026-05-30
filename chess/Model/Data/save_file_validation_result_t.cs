// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Data;

public sealed class save_file_validation_result_t {
    private save_file_validation_result_t(bool is_valid, string message, serialization_format_t? format) {
        this.is_valid = is_valid;
        get_message = message;
        get_format = format;
    }

    public bool is_valid { get; }

    public string get_message { get; }

    public serialization_format_t? get_format { get; }

    public static save_file_validation_result_t create_valid(serialization_format_t format) => new save_file_validation_result_t(true, "Save file is valid.", format);

    public static save_file_validation_result_t create_invalid(string message) => new save_file_validation_result_t(false, message, null);
}