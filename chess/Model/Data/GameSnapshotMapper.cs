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
            Pieces = [.. game.Board.pieces_getall()
                .Select(piece => new PieceSnapshot {
                    Type = piece.get_type.ToString(),
                    Color = piece.get_color,
                    Row = piece.get_position.row,
                    Column = piece.get_position.column,
                    MoveCount = piece.get_move_count,
                })],
            Moves = [.. game.MoveHistory
                .Select(move => new MoveSnapshot {
                    PieceType = move.get_piece_type,
                    PieceColor = move.get_piece_color,
                    FromRow = move.get_position_from.row,
                    FromColumn = move.get_position_from.column,
                    ToRow = move.get_position_to.row,
                    ToColumn = move.get_position_to.column,
                    CapturedPieceType = move.get_captured_piece_type?.ToString(),
                })],
        };
    }

    public static ChessGame ToGame(GameSnapshot snapshot) {
        var board = new board_t();
        foreach (var pieceSnapshot in snapshot.Pieces) {
            board.piece_place(CreatePiece(pieceSnapshot));
        }

        var moves = snapshot.Moves.Select(move => new piece_move_t(
            move.PieceType,
            move.PieceColor,
            new position_t(move.FromRow, move.FromColumn),
            new position_t(move.ToRow, move.ToColumn),
            string.IsNullOrWhiteSpace(move.CapturedPieceType)
                ? null
                : Enum.Parse<piece_type_t>(move.CapturedPieceType)));

        return new ChessGame(board, snapshot.CurrentTurn, moves);
    }

    private static piece_t CreatePiece(PieceSnapshot snapshot) {
        var position = new position_t(snapshot.Row, snapshot.Column);
        return Enum.Parse<piece_type_t>(snapshot.Type) switch {
            piece_type_t.PIECE_KING => new king_piece_t(snapshot.Color, position, snapshot.MoveCount),
            piece_type_t.PIECE_QUEEN => new Queen(snapshot.Color, position, snapshot.MoveCount),
            piece_type_t.PIECE_ROOK => new Rook(snapshot.Color, position, snapshot.MoveCount),
            piece_type_t.PIECE_BISHOP => new Bishop(snapshot.Color, position, snapshot.MoveCount),
            piece_type_t.PIECE_KNIGHT => new knight_piece_t(snapshot.Color, position, snapshot.MoveCount),
            piece_type_t.PIECE_PAWN => new pawn_piece_t(snapshot.Color, position, snapshot.MoveCount),
            _ => throw new InvalidOperationException($"Unknown piece type: {snapshot.Type}"),
        };
    }
}
