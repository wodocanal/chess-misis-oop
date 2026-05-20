namespace Model.Data;

public static class SaveFileValidator {
    public static SaveFileValidationResult Validate(string filePath, IEnumerable<GameSerializer> serializers) {
        if (string.IsNullOrWhiteSpace(filePath)) {
            return SaveFileValidationResult.Invalid("Не указан путь к файлу.");
        }

        if (!File.Exists(filePath)) {
            return SaveFileValidationResult.Invalid("Указанный файл не существует.");
        }

        var serializer = serializers.FirstOrDefault(candidate => candidate.CanRead(filePath));
        if (serializer is null) {
            return SaveFileValidationResult.Invalid("Формат файла не поддерживается. Используйте JSON или XML.");
        }

        try {
            serializer.Load(filePath);
            return SaveFileValidationResult.Valid(serializer.Format);
        } catch {
            return SaveFileValidationResult.Invalid("Файл не соответствует ожидаемому формату сохранения шахматной партии.");
        }
    }
}
