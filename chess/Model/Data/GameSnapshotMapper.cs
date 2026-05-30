// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Model.Core;

namespace Model.Data;

internal static class GameSnapshotMapper {
    public static game_snapshot_t ToSnapshot(chess_game_t game) {
        return new game_snapshot_t {
            get_current_turn = game.current_turn,
            get_status = game.status,
            get_pieces = [.. game.board.pieces_getall()
                .Select(piece => new piece_snapshot_t {
                    type = piece.get_type.ToString(),
                    color = piece.get_color,
                    row = piece.get_position.row,
                    column = piece.get_position.column,
                    move_count = piece.get_move_count,
                })],
            get_moves = [.. game.get_move_history
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

    public static chess_game_t ToGame(game_snapshot_t snapshot) {
        var board = new board_t();
        foreach (var pieceSnapshot in snapshot.get_pieces) {
            board.piece_place(CreatePiece(pieceSnapshot));
        }

        var moves = snapshot.get_moves.Select(move => new piece_move_t(
            move.PieceType,
            move.PieceColor,
            new position_t(move.FromRow, move.FromColumn),
            new position_t(move.ToRow, move.ToColumn),
            string.IsNullOrWhiteSpace(move.CapturedPieceType)
                ? null
                : Enum.Parse<piece_type_t>(move.CapturedPieceType)));

        return new chess_game_t(board, snapshot.get_current_turn, moves);
    }

    private static piece_t CreatePiece(piece_snapshot_t snapshot) {
        var position = new position_t(snapshot.row, snapshot.column);
        return Enum.Parse<piece_type_t>(snapshot.type) switch {
            piece_type_t.PIECE_TYPE_KING => new king_piece_t(snapshot.color, position, snapshot.move_count),
            piece_type_t.PIECE_TYPE_QUEEN => new queen_piece_t(snapshot.color, position, snapshot.move_count),
            piece_type_t.PIECE_TYPE_ROOK => new rook_piece_t(snapshot.color, position, snapshot.move_count),
            piece_type_t.PIECE_TYPE_BISHOP => new bishop_piece_t(snapshot.color, position, snapshot.move_count),
            piece_type_t.PIECE_TYPE_KNIGHT => new knight_piece_t(snapshot.color, position, snapshot.move_count),
            piece_type_t.PIECE_TYPE_PAWN => new pawn_piece_t(snapshot.color, position, snapshot.move_count),
            _ => throw new InvalidOperationException($"Unknown piece type: {snapshot.type}"),
        };
    }
}
