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
    private readonly IamInterfaceThatReperentsThatThisIsGameSerializer[] _serializers =
    [
        new JsonGameSerializer(),
        new XmlGameSerializer(),
    ];

    public MainWindow() {
        InitializeComponent();
        InitializeControls();
    }

    private void InitializeControls() {
        FormatComboBox.ItemsSource = Enum.GetValues<serialization_format_t>();
        FormatComboBox.SelectedItem = serialization_format_t.SERIALIZATION_FORMAT_JSON;
        FormatComboBox.SelectionChanged += FormatComboBox_OnSelectionChanged;

        SaveFolderTextBox.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Chess");
        UpdateSuggestedSavePath();
    }

    private void NewGameButton_OnClick(object? sender, RoutedEventArgs e) {
        var game = chess_game_t.create_new_game();
        OpenGameWindow(game, GetSelectedSerializer(), GetSaveFilePath());
    }

    private void ContinueGameButton_OnClick(object? sender, RoutedEventArgs e) {
        var filePath = SaveFileTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(filePath)) {
            SetValidationMessage("Для продолжения игры укажите путь к существующему файлу сохранения.");
            return;
        }

        var validation = save_file_validator_t.Validate(filePath, _serializers);
        if (!validation.is_valid) {
            SetValidationMessage(validation.get_message);
            return;
        }

        try {
            var serializer = _serializers.First(candidate => candidate.get_format == validation.get_format);
            var game = serializer.load(filePath);
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

    private void OpenGameWindow(chess_game_t game, IamInterfaceThatReperentsThatThisIsGameSerializer serializer, string saveFilePath) {
        var gameWindow = new GameWindow(game, serializer, saveFilePath);
        gameWindow.Closed += (_, _) => {
            Show();
            SaveFileTextBox.Text = saveFilePath;
            SetValidationMessage("Окно партии закрыто. При наличии ходов партия сохранена автоматически.");
        };

        gameWindow.Show();
        Hide();
    }

    private IamInterfaceThatReperentsThatThisIsGameSerializer GetSelectedSerializer() {
        var selected = FormatComboBox.SelectedItem as serialization_format_t?
            ?? serialization_format_t.SERIALIZATION_FORMAT_JSON;
        return _serializers.First(serializer => serializer.get_format == selected);
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

        return Path.Combine(folderPath, $"autosave{GetSelectedSerializer().get_file_extension}");
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
