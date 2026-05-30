// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Data;

public static class save_file_validator_t {
    public static save_file_validation_result_t Validate(string file_path, IEnumerable<IamInterfaceThatReperentsThatThisIsGameSerializer> serializers) {
        if (string.IsNullOrWhiteSpace(file_path)) {
            return save_file_validation_result_t.create_invalid("Не указан путь к файлу.");
        }

        if (!File.Exists(file_path)) {
            return save_file_validation_result_t.create_invalid("Указанный файл не существует.");
        }

        var serializer = serializers.FirstOrDefault(candidate => candidate.can_read(file_path));
        if (serializer is null) {
            return save_file_validation_result_t.create_invalid("Формат файла не поддерживается. Используйте JSON или XML.");
        }

        try {
            serializer.load(file_path);
            return save_file_validation_result_t.create_valid(serializer.get_format);
        } catch {
            return save_file_validation_result_t.create_invalid("Файл не соответствует ожидаемому формату сохранения шахматной партии.");
        }
    }
}
