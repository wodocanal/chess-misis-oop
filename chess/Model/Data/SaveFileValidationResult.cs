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

    public static SaveFileValidationResult Valid(SerializationFormat format) {
        return new SaveFileValidationResult(true, "Save file is valid.", format);
    }

    public static SaveFileValidationResult Invalid(string message) {
        return new SaveFileValidationResult(false, message, null);
    }
}
