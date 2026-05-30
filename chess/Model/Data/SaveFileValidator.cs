// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Data;

public static class SaveFileValidator {
    public static save_file_validation_result_t Validate(string filePath, IEnumerable<IamInterfaceThatReperentsThatThisIsGameSerializer> serializers) {
        if (string.IsNullOrWhiteSpace(filePath)) {
            return save_file_validation_result_t.make_invalid("Не указан путь к файлу.");
        }

        if (!File.Exists(filePath)) {
            return save_file_validation_result_t.make_invalid("Указанный файл не существует.");
        }

        var serializer = serializers.FirstOrDefault(candidate => candidate.can_read(filePath));
        if (serializer is null) {
            return save_file_validation_result_t.make_invalid("Формат файла не поддерживается. Используйте JSON или XML.");
        }

        try {
            serializer.load(filePath);
            return save_file_validation_result_t.make_valid(serializer.get_format);
        } catch {
            return save_file_validation_result_t.make_invalid("Файл не соответствует ожидаемому формату сохранения шахматной партии.");
        }
    }
}
