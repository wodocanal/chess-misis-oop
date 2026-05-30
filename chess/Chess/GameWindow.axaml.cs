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
    private readonly ChessGame _game;
    private readonly GameSerializer _serializer;
    private readonly string _saveFilePath;

    private Position? _selectedPosition;
    private IReadOnlyCollection<Position> _availableMoves = [];

    public GameWindow()
        : this(
            ChessGame.CreateNewGame(),
            new JsonGameSerializer(),
            Path.Combine(Path.GetTempPath(), "chess-preview.json")) {
    }

    public GameWindow(ChessGame game, GameSerializer serializer, string saveFilePath) {
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
                var position = new Position(row, column);
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

    private void HandleCellClick(Position position) {
        var clickedPiece = _game.Board.GetPiece(position);

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
            var result = _game.TryMove(_selectedPosition.Value, position);
            HandleMoveResult(result, _selectedPosition.Value, position);
            return;
        }

        if (clickedPiece is not null && clickedPiece.get_color == _game.CurrentTurn) {
            TrySelectPiece(position, clickedPiece);
            UpdateBoard();
            return;
        }

        UpdateSidebar("Эта клетка недоступна для выбранной фигуры.");
    }

    private bool TrySelectPiece(Position position, Piece? piece) {
        if (piece is null) {
            UpdateSidebar("На этой клетке нет фигуры.");
            return false;
        }

        if (piece.get_color != _game.CurrentTurn) {
            UpdateSidebar("Сейчас ходит другой цвет.");
            return false;
        }

        _selectedPosition = position;
        _availableMoves = _game.GetLegalMoves(position);
        UpdateSidebar(_availableMoves.Count == 0
            ? "Для выбранной фигуры сейчас нет допустимых ходов."
            : $"Выбрана фигура {GetPieceName(piece)} на {position}.");
        return true;
    }

    private void HandleMoveResult(MoveExecutionResult result, Position from, Position to) {
        switch (result) {
            case MoveExecutionResult.Success:
                _selectedPosition = null;
                _availableMoves = [];
                UpdateBoard();
                RefreshMoveLog();
                UpdateSidebar($"Ход выполнен: {from} -> {to}.");
                UpdateActionButtons();
                return;
            case MoveExecutionResult.CancelledSelection:
                ClearSelection("Выбор отменён.");
                return;
            case MoveExecutionResult.WrongTurn:
                UpdateSidebar("Сейчас ходит другой цвет.");
                return;
            case MoveExecutionResult.InvalidSource:
            case MoveExecutionResult.InvalidTarget:
                UpdateSidebar("Недопустимый ход.");
                return;
            case MoveExecutionResult.GameFinished:
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
                var position = new Position(row, column);
                var button = _boardButtons[row, column];
                var piece = _game.Board.GetPiece(position);

                button.Content = GetPieceSymbol(piece);
                button.Background = GetCellBackground(position);
                button.Foreground = piece?.get_color == PieceColor.White
                    ? Brushes.White
                    : Brushes.Black;
            }
        }
    }

    private void UpdateSidebar(string? detailMessage = null) {
        TurnTextBlock.Text = GetTurnLabel(_game.CurrentTurn);
        StateTextBlock.Text = CombineStateText(detailMessage);
        SavePathTextBlock.Text = _saveFilePath;
    }

    private void UpdateActionButtons() {
        UndoMoveButton.IsEnabled = _game.CanUndo;
    }

    private string CombineStateText(string? detailMessage) {
        var stateText = GetGameStateText();
        return string.IsNullOrWhiteSpace(detailMessage)
            ? stateText
            : $"{stateText} {detailMessage}";
    }

    private string GetGameStateText() {
        return _game.Status switch {
            GameStateStatus.InProgress => "Партия продолжается.",
            GameStateStatus.Check => $"Шах: под ударом {GetColorName(_game.CurrentTurn)} король.",
            GameStateStatus.Checkmate => $"Мат: {GetColorName(_game.CurrentTurn)} проиграли.",
            GameStateStatus.Stalemate => "Пат: партия завершена вничью.",
            _ => "Неизвестное состояние.",
        };
    }

    private IBrush GetCellBackground(Position position) {
        if (_selectedPosition == position) {
            return SelectedCellBrush;
        }

        if (_availableMoves.Contains(position)) {
            return AvailableMoveBrush;
        }

        return (position.Row + position.Column) % 2 == 0
            ? LightCellBrush
            : DarkCellBrush;
    }

    private void RefreshMoveLog() {
        MoveLogStackPanel.Children.Clear();

        if (_game.MoveHistory.Count == 0) {
            MoveLogStackPanel.Children.Add(CreateEmptyMoveLogState());
            return;
        }

        for (var index = _game.MoveHistory.Count - 1; index >= 0; index -= 1) {
            var move = _game.MoveHistory[index];
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

    private static Border CreateMoveLogEntry(Move move, int moveNumber) {
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

    private static string FormatMoveSummary(Move move) {
        var captureText = move.CapturedPieceType is null
            ? string.Empty
            : $", взята фигура: {GetPieceName(move.CapturedPieceType.Value)}";
        return $"{GetTurnLabel(move.PieceColor)}: {GetPieceName(move.PieceType)} {move.From} -> {move.To}{captureText}";
    }

    private static string GetPieceSymbol(Piece? piece) {
        if (piece is null) {
            return string.Empty;
        }

        return (piece.get_type, piece.get_color) switch {
            (PieceType.King, PieceColor.White) => "♔",
            (PieceType.Queen, PieceColor.White) => "♕",
            (PieceType.Rook, PieceColor.White) => "♖",
            (PieceType.Bishop, PieceColor.White) => "♗",
            (PieceType.Knight, PieceColor.White) => "♘",
            (PieceType.Pawn, PieceColor.White) => "♙",
            (PieceType.King, PieceColor.Black) => "♚",
            (PieceType.Queen, PieceColor.Black) => "♛",
            (PieceType.Rook, PieceColor.Black) => "♜",
            (PieceType.Bishop, PieceColor.Black) => "♝",
            (PieceType.Knight, PieceColor.Black) => "♞",
            (PieceType.Pawn, PieceColor.Black) => "♟",
            _ => piece.get_symbol,
        };
    }

    private static string GetPieceName(Piece piece) {
        return GetPieceName(piece.get_type);
    }

    private static string GetPieceName(PieceType pieceType) {
        return pieceType switch {
            PieceType.King => "король",
            PieceType.Queen => "ферзь",
            PieceType.Rook => "ладья",
            PieceType.Bishop => "слон",
            PieceType.Knight => "конь",
            PieceType.Pawn => "пешка",
            _ => "фигура",
        };
    }

    private static string GetTurnLabel(PieceColor color) {
        return color == PieceColor.White ? "Белые" : "Чёрные";
    }

    private static string GetColorName(PieceColor color) {
        return color == PieceColor.White ? "белый" : "чёрный";
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
        if (!_game.TryUndoLastMove()) {
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
