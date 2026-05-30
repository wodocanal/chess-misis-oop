// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

internal static class GameSnapshotMapper {
    public static GameSnapshot ToSnapshot(ChessGame game) {
        return new GameSnapshot {
            CurrentTurn = game.CurrentTurn,
            Status = game.Status,
            Pieces = [.. game.Board.GetPieces()
                .Select(piece => new PieceSnapshot {
                    Type = piece.get_type.ToString(),
                    Color = piece.get_color,
                    Row = piece.get_position.row,
                    Column = piece.get_position.column,
                    MoveCount = piece.get_move_count,
                })],
            Moves = [.. game.MoveHistory
                .Select(move => new MoveSnapshot {
                    PieceType = move.PieceType,
                    PieceColor = move.PieceColor,
                    FromRow = move.From.row,
                    FromColumn = move.From.column,
                    ToRow = move.To.row,
                    ToColumn = move.To.column,
                    CapturedPieceType = move.CapturedPieceType?.ToString(),
                })],
        };
    }

    public static ChessGame ToGame(GameSnapshot snapshot) {
        var board = new Board();
        foreach (var pieceSnapshot in snapshot.Pieces) {
            board.PlacePiece(CreatePiece(pieceSnapshot));
        }

        var moves = snapshot.Moves.Select(move => new Move(
            move.PieceType,
            move.PieceColor,
            new position_t(move.FromRow, move.FromColumn),
            new position_t(move.ToRow, move.ToColumn),
            string.IsNullOrWhiteSpace(move.CapturedPieceType)
                ? null
                : Enum.Parse<PieceType>(move.CapturedPieceType)));

        return new ChessGame(board, snapshot.CurrentTurn, moves);
    }

    private static Piece CreatePiece(PieceSnapshot snapshot) {
        var position = new position_t(snapshot.Row, snapshot.Column);
        return Enum.Parse<PieceType>(snapshot.Type) switch {
            PieceType.King => new King(snapshot.Color, position, snapshot.MoveCount),
            PieceType.Queen => new Queen(snapshot.Color, position, snapshot.MoveCount),
            PieceType.Rook => new Rook(snapshot.Color, position, snapshot.MoveCount),
            PieceType.Bishop => new Bishop(snapshot.Color, position, snapshot.MoveCount),
            PieceType.Knight => new Knight(snapshot.Color, position, snapshot.MoveCount),
            PieceType.Pawn => new Pawn(snapshot.Color, position, snapshot.MoveCount),
            _ => throw new InvalidOperationException($"Unknown piece type: {snapshot.Type}"),
        };
    }
}
