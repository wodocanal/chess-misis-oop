// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Data;

public static class SaveFileValidator {
    public static SaveFileValidationResult Validate(string filePath, IEnumerable<IamInterfaceThatReperentsThatThisIsGameSerializer> serializers) {
        if (string.IsNullOrWhiteSpace(filePath)) {
            return SaveFileValidationResult.Invalid("Не указан путь к файлу.");
        }

        if (!File.Exists(filePath)) {
            return SaveFileValidationResult.Invalid("Указанный файл не существует.");
        }

        var serializer = serializers.FirstOrDefault(candidate => candidate.can_read(filePath));
        if (serializer is null) {
            return SaveFileValidationResult.Invalid("Формат файла не поддерживается. Используйте JSON или XML.");
        }

        try {
            serializer.load(filePath);
            return SaveFileValidationResult.Valid(serializer.get_format);
        } catch {
            return SaveFileValidationResult.Invalid("Файл не соответствует ожидаемому формату сохранения шахматной партии.");
        }
    }
}
