// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

namespace Model.Core;

public partial class ChessGame {
    private readonly List<Move> _moveHistory = [];

    public event Action<GameStateStatus>? StateChanged;

    public ChessGame(Board board, PieceColor currentTurn, IEnumerable<Move>? moveHistory = null) {
        Board = board;
        CurrentTurn = currentTurn;
        if (moveHistory is not null) {
            _moveHistory.AddRange(moveHistory);
        }

        UpdateGameState();
    }

    public Board Board { get; private set; }

    public PieceColor CurrentTurn { get; private set; }

    public GameStateStatus Status { get; private set; }

    public IReadOnlyList<Move> MoveHistory => _moveHistory;

    public bool CanUndo => _moveHistory.Count > 0;

    public static ChessGame CreateNewGame() {
        var board = new Board();
        PlaceStartingPieces(board);
        return new ChessGame(board, PieceColor.White);
    }

    public IReadOnlyCollection<position_t> GetLegalMoves(position_t from) {
        var piece = Board.GetPiece(from);
        if (piece is null || piece.get_color != CurrentTurn) {
            return [];
        }

        return [.. piece.get_available_moves(Board).Where(target => WouldKeepKingSafe(Board, from, target, piece.get_color))];
    }

    public MoveExecutionResult TryMove(position_t from, position_t to) {
        if (Status is GameStateStatus.Checkmate or GameStateStatus.Stalemate) {
            return MoveExecutionResult.GameFinished;
        }

        if (from == to) {
            return MoveExecutionResult.CancelledSelection;
        }

        var movingPiece = Board.GetPiece(from);
        if (movingPiece is null) {
            return MoveExecutionResult.InvalidSource;
        }

        if (movingPiece.get_color != CurrentTurn) {
            return MoveExecutionResult.WrongTurn;
        }

        if (!GetLegalMoves(from).Contains(to)) {
            return MoveExecutionResult.InvalidTarget;
        }

        var capturedPiece = Board.GetPiece(to);
        Board.TryMove(from, to, out _);
        _moveHistory.Add(new Move(movingPiece.get_type, movingPiece.get_color, from, to, capturedPiece?.get_type));

        CurrentTurn = Toggle(CurrentTurn);
        UpdateGameState();
        return MoveExecutionResult.Success;
    }

    public MoveExecutionResult TryMove(Move move) => TryMove(move.From, move.To);

    public bool TryUndoLastMove() {
        if (!CanUndo) {
            return false;
        }

        _moveHistory.RemoveAt(_moveHistory.Count - 1);
        RebuildBoardFromHistory();
        return true;
    }

    public bool IsInCheck(PieceColor color) => IsKingInCheck(color, Board);

    internal bool HasAnyLegalMove(PieceColor color) {
        foreach (var piece in Board.GetPieces().Where(piece => piece.get_color == color)) {
            if (piece.get_available_moves(Board).Any(target => WouldKeepKingSafe(Board, piece.get_position, target, color))) {
                return true;
            }
        }

        return false;
    }

    internal bool IsInCheck(PieceColor color, Board board) => IsKingInCheck(color, board);

    private void UpdateGameState() {
        var previousStatus = Status;
        Status = DetermineStateFor(CurrentTurn);

        if (Status != previousStatus) {
            StateChanged?.Invoke(Status);
        }
    }

    private GameStateStatus DetermineStateFor(PieceColor color) {
        if (IsCheckmate(color)) {
            return GameStateStatus.Checkmate;
        }

        if (IsStalemate(color)) {
            return GameStateStatus.Stalemate;
        }

        return IsInCheck(color) ? GameStateStatus.Check : GameStateStatus.InProgress;
    }

    private static bool WouldKeepKingSafe(Board board, position_t from, position_t to, PieceColor movingColor) {
        var clonedBoard = board.Clone();
        clonedBoard.TryMove(from, to, out _);
        return !IsKingInCheck(movingColor, clonedBoard);
    }

    private static bool IsKingInCheck(PieceColor color, Board board) {
        var king = board.GetPieces<King>().FirstOrDefault(piece => piece.get_color == color);
        if (king is null) {
            return false;
        }

        return board.GetPieces()
            .Where(piece => piece.get_color != color)
            .Any(piece => piece.can_attack(king.get_position, board));
    }

    private static PieceColor Toggle(PieceColor color) => color == PieceColor.White ? PieceColor.Black : PieceColor.White;

    private void RebuildBoardFromHistory() {
        var rebuiltBoard = new Board();
        PlaceStartingPieces(rebuiltBoard);

        var currentTurn = PieceColor.White;
        foreach (var move in _moveHistory) {
            rebuiltBoard.TryMove(move.From, move.To, out _);
            currentTurn = Toggle(currentTurn);
        }

        Board = rebuiltBoard;
        CurrentTurn = currentTurn;
        UpdateGameState();
    }

    private static void PlaceStartingPieces(Board board) {
        for (var column = 0; column < 8; column += 1) {
            board.PlacePiece(new Pawn(PieceColor.White, new position_t(6, column)));
            board.PlacePiece(new Pawn(PieceColor.Black, new position_t(1, column)));
        }

        board.PlacePiece(new Rook(PieceColor.White, new position_t(7, 0)));
        board.PlacePiece(new Knight(PieceColor.White, new position_t(7, 1)));
        board.PlacePiece(new Bishop(PieceColor.White, new position_t(7, 2)));
        board.PlacePiece(new Queen(PieceColor.White, new position_t(7, 3)));
        board.PlacePiece(new King(PieceColor.White, new position_t(7, 4)));
        board.PlacePiece(new Bishop(PieceColor.White, new position_t(7, 5)));
        board.PlacePiece(new Knight(PieceColor.White, new position_t(7, 6)));
        board.PlacePiece(new Rook(PieceColor.White, new position_t(7, 7)));

        board.PlacePiece(new Rook(PieceColor.Black, new position_t(0, 0)));
        board.PlacePiece(new Knight(PieceColor.Black, new position_t(0, 1)));
        board.PlacePiece(new Bishop(PieceColor.Black, new position_t(0, 2)));
        board.PlacePiece(new Queen(PieceColor.Black, new position_t(0, 3)));
        board.PlacePiece(new King(PieceColor.Black, new position_t(0, 4)));
        board.PlacePiece(new Bishop(PieceColor.Black, new position_t(0, 5)));
        board.PlacePiece(new Knight(PieceColor.Black, new position_t(0, 6)));
        board.PlacePiece(new Rook(PieceColor.Black, new position_t(0, 7)));
    }
}
