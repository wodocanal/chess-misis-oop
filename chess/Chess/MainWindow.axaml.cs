// SPDX-License-Identifier: GPL-3.0-or-later
// SPDX-FileCopyrightText: 2026 Maxim Naumov (KiraFlux) <kiraflux@duck.com>
// SPDX-FileCopyrightText: 2026 Yuri Golyshev (wodocanal)

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Model.Core;
using Model.Data;

namespace Chess;

public partial class MainWindow : Window {
    private readonly GameSerializer[] _serializers =
    [
        new JsonGameSerializer(),
        new XmlGameSerializer(),
    ];

    public MainWindow() {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls() {
        FormatComboBox.ItemsSource = Enum.GetValues<SerializationFormat>();
        FormatComboBox.SelectedItem = SerializationFormat.Json;
        FormatComboBox.SelectionChanged += FormatComboBox_OnSelectionChanged;

        SaveFolderTextBox.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Chess");
        UpdateSuggestedSavePath();
    }

    private void NewGameButton_OnClick(object? sender, RoutedEventArgs e) {
        var game = chess_game_t.CreateNewGame();
        OpenGameWindow(game, GetSelectedSerializer(), GetSaveFilePath());
    }

    private void ContinueGameButton_OnClick(object? sender, RoutedEventArgs e) {
        var filePath = SaveFileTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(filePath)) {
            SetValidationMessage("Для продолжения игры укажите путь к существующему файлу сохранения.");
            return;
        }

        var validation = SaveFileValidator.Validate(filePath, _serializers);
        if (!validation.IsValid) {
            SetValidationMessage(validation.Message);
            return;
        }

        try {
            var serializer = _serializers.First(candidate => candidate.Format == validation.Format);
            var game = serializer.Load(filePath);
            OpenGameWindow(game, serializer, filePath);
        } catch (Exception exception) {
            SetValidationMessage($"Не удалось загрузить сохранение: {exception.Message}");
        }
    }

    private async void BrowseFolderButton_OnClick(object? sender, RoutedEventArgs e) {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {
            Title = "Выберите папку для сохранения",
            AllowMultiple = false,
        });

        var path = folders.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path)) {
            SaveFolderTextBox.Text = path;
            UpdateSuggestedSavePath();
            SetValidationMessage("Папка сохранения обновлена.");
        }
    }

    private async void BrowseFileButton_OnClick(object? sender, RoutedEventArgs e) {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
            Title = "Выберите файл сохранения",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Chess saves")
                {
                    Patterns = ["*.json", "*.xml"],
                },
            ],
        });

        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path)) {
            SaveFileTextBox.Text = path;
            SetValidationMessage("Файл сохранения выбран.");
        }
    }

    private void FormatComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        UpdateSuggestedSavePath();
    }

    private void OpenGameWindow(chess_game_t game, GameSerializer serializer, string saveFilePath) {
        var gameWindow = new GameWindow(game, serializer, saveFilePath);
        gameWindow.Closed += (_, _) => {
            Show();
            SaveFileTextBox.Text = saveFilePath;
            SetValidationMessage("Окно партии закрыто. При наличии ходов партия сохранена автоматически.");
        };

        gameWindow.Show();
        Hide();
    }

    private GameSerializer GetSelectedSerializer() {
        var selected = FormatComboBox.SelectedItem as SerializationFormat?
            ?? SerializationFormat.Json;
        return _serializers.First(serializer => serializer.Format == selected);
    }

    private string GetSaveFilePath() {
        var explicitPath = SaveFileTextBox.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(explicitPath)) {
            return explicitPath;
        }

        var folderPath = SaveFolderTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(folderPath)) {
            folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Chess");
        }

        return Path.Combine(folderPath, $"autosave{GetSelectedSerializer().FileExtension}");
    }

    private void UpdateSuggestedSavePath() {
        if (string.IsNullOrWhiteSpace(SaveFileTextBox.Text)) {
            SaveFileTextBox.Text = GetSaveFilePath();
        }
    }

    private void SetValidationMessage(string message) {
        ValidationTextBlock.Text = message;
    }
}
