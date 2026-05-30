// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public partial class ChessGame {
    private readonly List<piece_move_t> _moveHistory = [];

    public event Action<game_state_status_t>? StateChanged;

    public ChessGame(board_t board, piece_color_t currentTurn, IEnumerable<piece_move_t>? moveHistory = null) {
        Board = board;
        CurrentTurn = currentTurn;
        if (moveHistory is not null) {
            _moveHistory.AddRange(moveHistory);
        }

        UpdateGameState();
    }

    public board_t Board { get; private set; }

    public piece_color_t CurrentTurn { get; private set; }

    public game_state_status_t Status { get; private set; }

    public IReadOnlyList<piece_move_t> MoveHistory => _moveHistory;

    public bool CanUndo => _moveHistory.Count > 0;

    public static ChessGame CreateNewGame() {
        var board = new board_t();
        PlaceStartingPieces(board);
        return new ChessGame(board, piece_color_t.PIECE_COLOR_WHITE);
    }

    public IReadOnlyCollection<position_t> GetLegalMoves(position_t from) {
        var piece = Board.piece_get(from);
        if (piece is null || piece.get_color != CurrentTurn) {
            return [];
        }

        return [.. piece.get_available_moves(Board).Where(target => WouldKeepKingSafe(Board, from, target, piece.get_color))];
    }

    public move_execution_result_t TryMove(position_t from, position_t to) {
        if (Status is game_state_status_t.GAME_STATUS_IN_CHECKMATE or game_state_status_t.GAME_STATUS_STALEMATE) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_GAME_FINISHED;
        }

        if (from == to) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_CANCELLED_SELECTION;
        }

        var movingPiece = Board.piece_get(from);
        if (movingPiece is null) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_INVALID_SOURCE;
        }

        if (movingPiece.get_color != CurrentTurn) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_WRONG_TURN;
        }

        if (!GetLegalMoves(from).Contains(to)) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_INVALID_TARGET;
        }

        var capturedPiece = Board.piece_get(to);
        Board.try_move(from, to, out _);
        _moveHistory.Add(new piece_move_t(movingPiece.get_type, movingPiece.get_color, from, to, capturedPiece?.get_type));

        CurrentTurn = Toggle(CurrentTurn);
        UpdateGameState();
        return move_execution_result_t.MOVE_EXECUTION_RESULT_SUCCESS;
    }

    public move_execution_result_t TryMove(piece_move_t move) => TryMove(move.get_position_from, move.get_position_to);

    public bool TryUndoLastMove() {
        if (!CanUndo) {
            return false;
        }

        _moveHistory.RemoveAt(_moveHistory.Count - 1);
        RebuildBoardFromHistory();
        return true;
    }

    public bool IsInCheck(piece_color_t color) => IsKingInCheck(color, Board);

    internal bool HasAnyLegalMove(piece_color_t color) {
        foreach (var piece in Board.pieces_getall().Where(piece => piece.get_color == color)) {
            if (piece.get_available_moves(Board).Any(target => WouldKeepKingSafe(Board, piece.get_position, target, color))) {
                return true;
            }
        }

        return false;
    }

    internal bool IsInCheck(piece_color_t color, board_t board) => IsKingInCheck(color, board);

    private void UpdateGameState() {
        var previousStatus = Status;
        Status = DetermineStateFor(CurrentTurn);

        if (Status != previousStatus) {
            StateChanged?.Invoke(Status);
        }
    }

    private game_state_status_t DetermineStateFor(piece_color_t color) {
        if (IsCheckmate(color)) {
            return game_state_status_t.GAME_STATUS_IN_CHECKMATE;
        }

        if (IsStalemate(color)) {
            return game_state_status_t.GAME_STATUS_STALEMATE;
        }

        return IsInCheck(color) ? game_state_status_t.GAME_STATUS_CHECK : game_state_status_t.GAME_STATUS_IN_PROGRESS;
    }

    private static bool WouldKeepKingSafe(board_t board, position_t from, position_t to, piece_color_t movingColor) {
        var clonedBoard = board.make_clone();
        clonedBoard.try_move(from, to, out _);
        return !IsKingInCheck(movingColor, clonedBoard);
    }

    private static bool IsKingInCheck(piece_color_t color, board_t board) {
        var king = board.pieces_getall<king_piece_t>().FirstOrDefault(piece => piece.get_color == color);
        if (king is null) {
            return false;
        }

        return board.pieces_getall()
            .Where(piece => piece.get_color != color)
            .Any(piece => piece.can_attack(king.get_position, board));
    }

    private static piece_color_t Toggle(piece_color_t color) => color == piece_color_t.PIECE_COLOR_WHITE ? piece_color_t.PIECE_COLOR_BLACK : piece_color_t.PIECE_COLOR_WHITE;

    private void RebuildBoardFromHistory() {
        var rebuiltBoard = new board_t();
        PlaceStartingPieces(rebuiltBoard);

        var currentTurn = piece_color_t.PIECE_COLOR_WHITE;
        foreach (var move in _moveHistory) {
            rebuiltBoard.try_move(move.get_position_from, move.get_position_to, out _);
            currentTurn = Toggle(currentTurn);
        }

        Board = rebuiltBoard;
        CurrentTurn = currentTurn;
        UpdateGameState();
    }

    private static void PlaceStartingPieces(board_t board) {
        for (var column = 0; column < 8; column += 1) {
            board.piece_place(new pawn_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(6, column)));
            board.piece_place(new pawn_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(1, column)));
        }

        board.piece_place(new rook_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 0)));
        board.piece_place(new knight_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 1)));
        board.piece_place(new bishop_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 2)));
        board.piece_place(new queen_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 3)));
        board.piece_place(new king_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 4)));
        board.piece_place(new bishop_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 5)));
        board.piece_place(new knight_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 6)));
        board.piece_place(new rook_piece_t(piece_color_t.PIECE_COLOR_WHITE, new position_t(7, 7)));

        board.piece_place(new rook_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 0)));
        board.piece_place(new knight_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 1)));
        board.piece_place(new bishop_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 2)));
        board.piece_place(new queen_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 3)));
        board.piece_place(new king_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 4)));
        board.piece_place(new bishop_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 5)));
        board.piece_place(new knight_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 6)));
        board.piece_place(new rook_piece_t(piece_color_t.PIECE_COLOR_BLACK, new position_t(0, 7)));
    }
}
