// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public class chess_game_t {
    private readonly List<piece_move_t> move_history = [];

    public event Action<game_state_status_t>? StateChanged;

    public chess_game_t(board_t board, piece_color_t current_turn, IEnumerable<piece_move_t>? move_history = null) {
        this.board = board;
        this.current_turn = current_turn;
        if (move_history is not null) {
            this.move_history.AddRange(move_history);
        }

        update_game_state();
    }

    public board_t board { get; private set; }

    public piece_color_t current_turn { get; private set; }

    public game_state_status_t status { get; private set; }

    public IReadOnlyList<piece_move_t> get_move_history => this.move_history;

    public bool can_undo => this.move_history.Count > 0;

    public static chess_game_t create_new_game() {
        var board = new board_t();
        place_starting_pieces(board);
        return new chess_game_t(board, piece_color_t.PIECE_COLOR_WHITE);
    }

    public IReadOnlyCollection<position_t> get_legal_moves(position_t from) {
        var piece = board.piece_get(from);
        if (piece is null || piece.get_color != current_turn) {
            return [];
        }

        return [.. piece.get_available_moves(board).Where(target => would_keep_king_safe(board, from, target, piece.get_color))];
    }

    public move_execution_result_t try_move(position_t from, position_t to) {
        if (status is game_state_status_t.GAME_STATUS_IN_CHECKMATE or game_state_status_t.GAME_STATUS_STALEMATE) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_GAME_FINISHED;
        }

        if (from == to) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_CANCELLED_SELECTION;
        }

        var movingPiece = board.piece_get(from);
        if (movingPiece is null) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_INVALID_SOURCE;
        }

        if (movingPiece.get_color != current_turn) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_WRONG_TURN;
        }

        if (!get_legal_moves(from).Contains(to)) {
            return move_execution_result_t.MOVE_EXECUTION_RESULT_INVALID_TARGET;
        }

        var captured_piece = board.piece_get(to);
        board.try_move(from, to, out _);
        this.move_history.Add(new piece_move_t(movingPiece.get_type, movingPiece.get_color, from, to, captured_piece?.get_type));

        current_turn = toggle_colors(current_turn);
        update_game_state();
        return move_execution_result_t.MOVE_EXECUTION_RESULT_SUCCESS;
    }

    public move_execution_result_t TryMove(piece_move_t move) => try_move(move.get_position_from, move.get_position_to);

    public bool try_undo_last_move() {
        if (!can_undo) {
            return false;
        }

        this.move_history.RemoveAt(this.move_history.Count - 1);
        rebuild_board_from_history();
        return true;
    }

    public bool is_in_check(piece_color_t color) => is_king_in_check(color, board);

    internal bool has_any_legal_move(piece_color_t color) {
        foreach (var piece in board.pieces_getall().Where(piece => piece.get_color == color)) {
            if (piece.get_available_moves(board).Any(target => would_keep_king_safe(board, piece.get_position, target, color))) {
                return true;
            }
        }

        return false;
    }

    internal bool is_in_check(piece_color_t color, board_t board) => is_king_in_check(color, board);

    private void update_game_state() {
        var previousStatus = status;
        status = determine_state_for(current_turn);

        if (status != previousStatus) {
            StateChanged?.Invoke(status);
        }
    }

    private game_state_status_t determine_state_for(piece_color_t color) {
        if (is_checkmate(color)) {
            return game_state_status_t.GAME_STATUS_IN_CHECKMATE;
        }

        if (is_stalemate(color)) {
            return game_state_status_t.GAME_STATUS_STALEMATE;
        }

        return is_in_check(color) ? game_state_status_t.GAME_STATUS_CHECK : game_state_status_t.GAME_STATUS_IN_PROGRESS;
    }

    private static bool would_keep_king_safe(board_t board, position_t from, position_t to, piece_color_t movingColor) {
        var clonedBoard = board.make_clone();
        clonedBoard.try_move(from, to, out _);
        return !is_king_in_check(movingColor, clonedBoard);
    }

    private static bool is_king_in_check(piece_color_t color, board_t board) {
        var king = board.pieces_getall<king_piece_t>().FirstOrDefault(piece => piece.get_color == color);
        if (king is null) { return false; }

        return board.pieces_getall().Where(piece => piece.get_color != color).Any(piece => piece.can_attack(king.get_position, board));
    }

    private static piece_color_t toggle_colors(piece_color_t color) =>
        color == piece_color_t.PIECE_COLOR_WHITE ? piece_color_t.PIECE_COLOR_BLACK : piece_color_t.PIECE_COLOR_WHITE;

    private void rebuild_board_from_history() {
        var rebuiltBoard = new board_t();
        place_starting_pieces(rebuiltBoard);

        var current_turn = piece_color_t.PIECE_COLOR_WHITE;
        foreach (var move in this.move_history) {
            rebuiltBoard.try_move(move.get_position_from, move.get_position_to, out _);
            current_turn = toggle_colors(current_turn);
        }

        board = rebuiltBoard;
        this.current_turn = current_turn;
        update_game_state();
    }
    private bool is_checkmate(piece_color_t color) => is_in_check(color) && !has_any_legal_move(color);

    private bool is_stalemate(piece_color_t color) {
        if (is_in_check(color)) {
            return false;
        }

        if (!has_any_legal_move(color)) {
            return true;
        }

        return is_six_reversible_half_moves_reached();
    }

    private bool is_six_reversible_half_moves_reached() {
        if (this.move_history.Count < 6) {
            return false;
        }

        var lastMoves = this.move_history.TakeLast(6).ToArray();
        for (var index = 1; index < lastMoves.Length; index += 2) {
            if (!lastMoves[index].is_reverse_of(lastMoves[index - 1])) {
                return false;
            }
        }

        return true;
    }

    private static void place_starting_pieces(board_t board) {
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
