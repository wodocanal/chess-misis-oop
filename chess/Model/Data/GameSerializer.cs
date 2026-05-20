using Model.Core;

namespace Model.Data;

public interface GameSerializer {
    SerializationFormat Format { get; }

    string FileExtension { get; }

    bool CanRead(string filePath);

    void Save(ChessGame game, string filePath);

    ChessGame Load(string filePath);
}
