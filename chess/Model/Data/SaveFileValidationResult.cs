// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Data;

public sealed class SaveFileValidationResult {
    private SaveFileValidationResult(bool isValid, string message, SerializationFormat? format) {
        IsValid = isValid;
        Message = message;
        Format = format;
    }

    public bool IsValid { get; }

    public string Message { get; }

    public SerializationFormat? Format { get; }

    public static SaveFileValidationResult Valid(SerializationFormat format) => new SaveFileValidationResult(true, "Save file is valid.", format);

    public static SaveFileValidationResult Invalid(string message) => new SaveFileValidationResult(false, message, null);
}
