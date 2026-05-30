// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Model.Core;
using Model.Data;

namespace Chess;

public partial class GameWindow : Window {
    private const int BoardDimension = 8;
    private const double CellSize = 67;
    private const double CellGap = 5;
    private const double CoordinateSize = 30;
    private const double CoordinateGap = 8;
    private const double BoardShellPadding = 18;
    private const double BoardSize = CellSize * BoardDimension + CellGap * (BoardDimension - 1);
    private const double BoardShellHeight = BoardSize + CoordinateSize * 2 + CoordinateGap * 2 + BoardShellPadding * 2;

    private static readonly IBrush DarkCellBrush = Brush.Parse("#8C5E3C");
    private static readonly IBrush LightCellBrush = Brush.Parse("#E8D8B6");
    private static readonly IBrush SelectedCellBrush = Brush.Parse("#E9B44C");
    private static readonly IBrush AvailableMoveBrush = Brush.Parse("#7FB685");

    private readonly Button[,] _boardButtons = new Button[8, 8];
    private readonly chess_game_t _game;
    private readonly GameSerializer _serializer;
    private readonly string _saveFilePath;

    private position_t? _selectedPosition;
    private IReadOnlyCollection<position_t> _availableMoves = [];

    public GameWindow()
        : this(
            chess_game_t.create_new_game(),
            new JsonGameSerializer(),
            Path.Combine(Path.GetTempPath(), "chess-preview.json")) {
    }

    public GameWindow(chess_game_t game, GameSerializer serializer, string saveFilePath) {
        _game = game;
        _serializer = serializer;
        _saveFilePath = saveFilePath;

        InitializeComponent();
        BoardShellBorder.Height = BoardShellHeight;
        SidebarBorder.Height = BoardShellHeight;
        BuildCoordinates();
        BuildBoard();
        UpdateBoard();
        UpdateSidebar();
        RefreshMoveLog();
        UpdateActionButtons();

        Closing += GameWindow_OnClosing;
    }

    private void BuildBoard() {
        BoardGrid.Width = BoardSize;
        BoardGrid.Height = BoardSize;
        BoardGrid.RowSpacing = CellGap;
        BoardGrid.ColumnSpacing = CellGap;
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();
        BoardGrid.Children.Clear();

        for (var index = 0; index < BoardDimension; index += 1) {
            BoardGrid.RowDefinitions.Add(new RowDefinition(new GridLength(CellSize)));
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(CellSize)));
        }

        for (var row = 0; row < BoardDimension; row += 1) {
            for (var column = 0; column < BoardDimension; column += 1) {
                var position = new position_t(row, column);
                var button = new Button {
                    Width = CellSize,
                    Height = CellSize,
                    FontSize = 34,
                    FontWeight = FontWeight.Bold,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                };
                button.Click += (_, _) => HandleCellClick(position);

                _boardButtons[row, column] = button;
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);
                BoardGrid.Children.Add(button);
            }
        }
    }

    private void BuildCoordinates() {
        BuildHorizontalCoordinates(TopCoordinatesGrid);
        BuildHorizontalCoordinates(BottomCoordinatesGrid);
        BuildVerticalCoordinates(LeftCoordinatesGrid);
        BuildVerticalCoordinates(RightCoordinatesGrid);
    }

    private void BuildHorizontalCoordinates(Grid targetGrid) {
        targetGrid.Width = BoardSize;
        targetGrid.Height = CoordinateSize;
        targetGrid.ColumnSpacing = CellGap;
        targetGrid.ColumnDefinitions.Clear();
        targetGrid.Children.Clear();

        for (var column = 0; column < BoardDimension; column += 1) {
            targetGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(CellSize)));

            var label = CreateHorizontalCoordinateLabel(((char)('A' + column)).ToString());
            Grid.SetColumn(label, column);
            targetGrid.Children.Add(label);
        }
    }

    private void BuildVerticalCoordinates(Grid targetGrid) {
        targetGrid.Width = CoordinateSize;
        targetGrid.Height = BoardSize;
        targetGrid.RowSpacing = CellGap;
        targetGrid.RowDefinitions.Clear();
        targetGrid.Children.Clear();

        for (var row = 0; row < BoardDimension; row += 1) {
            targetGrid.RowDefinitions.Add(new RowDefinition(new GridLength(CellSize)));

            var label = CreateVerticalCoordinateLabel((BoardDimension - row).ToString());
            Grid.SetRow(label, row);
            targetGrid.Children.Add(label);
        }
    }

    private static TextBlock CreateHorizontalCoordinateLabel(string text) {
        return new TextBlock {
            Text = text,
            Width = CellSize,
            Height = CoordinateSize,
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
    }

    private static TextBlock CreateVerticalCoordinateLabel(string text) {
        return new TextBlock {
            Text = text,
            Width = CoordinateSize,
            Height = CellSize,
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
    }

    private void HandleCellClick(position_t position) {
        var clickedPiece = _game.board.piece_get(position);

        if (_selectedPosition is null) {
            if (!TrySelectPiece(position, clickedPiece)) {
                return;
            }

            UpdateBoard();
            return;
        }

        if (_selectedPosition == position) {
            ClearSelection("Выбор отменён. Можно выбрать другую фигуру.");
            return;
        }

        if (_availableMoves.Contains(position)) {
            var result = _game.try_move(_selectedPosition.Value, position);
            HandleMoveResult(result, _selectedPosition.Value, position);
            return;
        }

        if (clickedPiece is not null && clickedPiece.get_color == _game.current_turn) {
            TrySelectPiece(position, clickedPiece);
            UpdateBoard();
            return;
        }

        UpdateSidebar("Эта клетка недоступна для выбранной фигуры.");
    }

    private bool TrySelectPiece(position_t position, piece_t? piece) {
        if (piece is null) {
            UpdateSidebar("На этой клетке нет фигуры.");
            return false;
        }

        if (piece.get_color != _game.current_turn) {
            UpdateSidebar("Сейчас ходит другой цвет.");
            return false;
        }

        _selectedPosition = position;
        _availableMoves = _game.get_legal_moves(position);
        UpdateSidebar(_availableMoves.Count == 0
            ? "Для выбранной фигуры сейчас нет допустимых ходов."
            : $"Выбрана фигура {GetPieceName(piece)} на {position}.");
        return true;
    }

    private void HandleMoveResult(move_execution_result_t result, position_t from, position_t to) {
        switch (result) {
            case move_execution_result_t.MOVE_EXECUTION_RESULT_SUCCESS:
                _selectedPosition = null;
                _availableMoves = [];
                UpdateBoard();
                RefreshMoveLog();
                UpdateSidebar($"Ход выполнен: {from} -> {to}.");
                UpdateActionButtons();
                return;
            case move_execution_result_t.MOVE_EXECUTION_RESULT_CANCELLED_SELECTION:
                ClearSelection("Выбор отменён.");
                return;
            case move_execution_result_t.MOVE_EXECUTION_RESULT_WRONG_TURN:
                UpdateSidebar("Сейчас ходит другой цвет.");
                return;
            case move_execution_result_t.MOVE_EXECUTION_RESULT_INVALID_SOURCE:
            case move_execution_result_t.MOVE_EXECUTION_RESULT_INVALID_TARGET:
                UpdateSidebar("Недопустимый ход.");
                return;
            case move_execution_result_t.MOVE_EXECUTION_RESULT_GAME_FINISHED:
                UpdateSidebar("Партия уже завершена.");
                return;
            default:
                UpdateSidebar("Не удалось выполнить ход.");
                return;
        }
    }

    private void UpdateBoard() {
        for (var row = 0; row < BoardDimension; row += 1) {
            for (var column = 0; column < BoardDimension; column += 1) {
                var position = new position_t(row, column);
                var button = _boardButtons[row, column];
                var piece = _game.board.piece_get(position);

                button.Content = GetPieceSymbol(piece);
                button.Background = GetCellBackground(position);
                button.Foreground = piece?.get_color == piece_color_t.PIECE_COLOR_WHITE
                    ? Brushes.White
                    : Brushes.Black;
            }
        }
    }

    private void UpdateSidebar(string? detailMessage = null) {
        TurnTextBlock.Text = GetTurnLabel(_game.current_turn);
        StateTextBlock.Text = CombineStateText(detailMessage);
        SavePathTextBlock.Text = _saveFilePath;
    }

    private void UpdateActionButtons() {
        UndoMoveButton.IsEnabled = _game.can_undo;
    }

    private string CombineStateText(string? detailMessage) {
        var stateText = GetGameStateText();
        return string.IsNullOrWhiteSpace(detailMessage)
            ? stateText
            : $"{stateText} {detailMessage}";
    }

    private string GetGameStateText() {
        return _game.status switch {
            game_state_status_t.GAME_STATUS_IN_PROGRESS => "Партия продолжается.",
            game_state_status_t.GAME_STATUS_CHECK => $"Шах: под ударом {GetColorName(_game.current_turn)} король.",
            game_state_status_t.GAME_STATUS_IN_CHECKMATE => $"Мат: {GetColorName(_game.current_turn)} проиграли.",
            game_state_status_t.GAME_STATUS_STALEMATE => "Пат: партия завершена вничью.",
            _ => "Неизвестное состояние.",
        };
    }

    private IBrush GetCellBackground(position_t position) {
        if (_selectedPosition == position) {
            return SelectedCellBrush;
        }

        if (_availableMoves.Contains(position)) {
            return AvailableMoveBrush;
        }

        return (position.row + position.column) % 2 == 0
            ? LightCellBrush
            : DarkCellBrush;
    }

    private void RefreshMoveLog() {
        MoveLogStackPanel.Children.Clear();

        if (_game.get_move_history.Count == 0) {
            MoveLogStackPanel.Children.Add(CreateEmptyMoveLogState());
            return;
        }

        for (var index = _game.get_move_history.Count - 1; index >= 0; index -= 1) {
            var move = _game.get_move_history[index];
            MoveLogStackPanel.Children.Add(CreateMoveLogEntry(move, index + 1));
        }
    }

    private static Border CreateEmptyMoveLogState() {
        return new Border {
            Padding = new Thickness(12),
            Background = Brush.Parse("#FBF7F0"),
            BorderBrush = Brush.Parse("#D9C8AA"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Child = new TextBlock {
                Text = "Журнал пока пуст. Первый ход появится здесь автоматически.",
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
            },
        };
    }

    private static Border CreateMoveLogEntry(piece_move_t move, int moveNumber) {
        return new Border {
            Padding = new Thickness(12),
            Background = Brush.Parse("#FBF7F0"),
            BorderBrush = Brush.Parse("#D9C8AA"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Child = new TextBlock {
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Text = $"{moveNumber}. {FormatMoveSummary(move)}",
            },
        };
    }

    private static string FormatMoveSummary(piece_move_t move) {
        var captureText = move.get_captured_piece_type is null
            ? string.Empty
            : $", взята фигура: {GetPieceName(move.get_captured_piece_type.Value)}";
        return $"{GetTurnLabel(move.get_piece_color)}: {GetPieceName(move.get_piece_type)} {move.get_position_from} -> {move.get_position_to}{captureText}";
    }

    private static string GetPieceSymbol(piece_t? piece) {
        if (piece is null) {
            return string.Empty;
        }

        return (piece.get_type, piece.get_color) switch {
            (piece_type_t.PIECE_KING, piece_color_t.PIECE_COLOR_WHITE) => "♔",
            (piece_type_t.PIECE_QUEEN, piece_color_t.PIECE_COLOR_WHITE) => "♕",
            (piece_type_t.PIECE_ROOK, piece_color_t.PIECE_COLOR_WHITE) => "♖",
            (piece_type_t.PIECE_BISHOP, piece_color_t.PIECE_COLOR_WHITE) => "♗",
            (piece_type_t.PIECE_KNIGHT, piece_color_t.PIECE_COLOR_WHITE) => "♘",
            (piece_type_t.PIECE_PAWN, piece_color_t.PIECE_COLOR_WHITE) => "♙",
            (piece_type_t.PIECE_KING, piece_color_t.PIECE_COLOR_BLACK) => "♚",
            (piece_type_t.PIECE_QUEEN, piece_color_t.PIECE_COLOR_BLACK) => "♛",
            (piece_type_t.PIECE_ROOK, piece_color_t.PIECE_COLOR_BLACK) => "♜",
            (piece_type_t.PIECE_BISHOP, piece_color_t.PIECE_COLOR_BLACK) => "♝",
            (piece_type_t.PIECE_KNIGHT, piece_color_t.PIECE_COLOR_BLACK) => "♞",
            (piece_type_t.PIECE_PAWN, piece_color_t.PIECE_COLOR_BLACK) => "♟",
            _ => piece.get_symbol,
        };
    }

    private static string GetPieceName(piece_t piece) {
        return GetPieceName(piece.get_type);
    }

    private static string GetPieceName(piece_type_t pieceType) {
        return pieceType switch {
            piece_type_t.PIECE_KING => "король",
            piece_type_t.PIECE_QUEEN => "ферзь",
            piece_type_t.PIECE_ROOK => "ладья",
            piece_type_t.PIECE_BISHOP => "слон",
            piece_type_t.PIECE_KNIGHT => "конь",
            piece_type_t.PIECE_PAWN => "пешка",
            _ => "фигура",
        };
    }

    private static string GetTurnLabel(piece_color_t color) {
        return color == piece_color_t.PIECE_COLOR_WHITE ? "Белые" : "Чёрные";
    }

    private static string GetColorName(piece_color_t color) {
        return color == piece_color_t.PIECE_COLOR_WHITE ? "белый" : "чёрный";
    }

    private void ClearSelection(string message) {
        _selectedPosition = null;
        _availableMoves = [];
        UpdateSidebar(message);
        UpdateBoard();
    }

    private void SaveNowButton_OnClick(object? sender, RoutedEventArgs e) {
        SaveGame();
        UpdateSidebar($"Партия сохранена в {_saveFilePath}.");
    }

    private void UndoMoveButton_OnClick(object? sender, RoutedEventArgs e) {
        if (!_game.try_undo_last_move()) {
            UpdateSidebar("Пока нет ходов для отмены.");
            return;
        }

        _selectedPosition = null;
        _availableMoves = [];
        UpdateBoard();
        RefreshMoveLog();
        UpdateSidebar("Последний ход отменён.");
        UpdateActionButtons();
    }

    private void BackToMenuButton_OnClick(object? sender, RoutedEventArgs e) {
        Close();
    }

    private void GameWindow_OnClosing(object? sender, WindowClosingEventArgs e) {
        SaveGame();
    }

    private void SaveGame() {
        _serializer.Save(_game, _saveFilePath);
    }
}
