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
    private readonly IamInterfaceThatReperentsThatThisIsGameSerializer[] serializers =
    [
        new json_game_serializer_t(),
        new xml_game_serializer_t(),
    ];

    public MainWindow() {
        InitializeComponent();
        initialize_controls();
    }

    private void initialize_controls() {
        FormatComboBox.ItemsSource = Enum.GetValues<serialization_format_t>();
        FormatComboBox.SelectedItem = serialization_format_t.SERIALIZATION_FORMAT_JSON;
        FormatComboBox.SelectionChanged += FormatComboBox_OnSelectionChanged;

        SaveFolderTextBox.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Chess");
        update_suggested_save_path();
    }

    private void NewGameButton_OnClick(object? sender, RoutedEventArgs e) {
        var game = chess_game_t.create_new_game();
        open_game_window(game, get_selected_serializer(), get_save_file_path());
    }

    private void ContinueGameButton_OnClick(object? sender, RoutedEventArgs e) {
        var filePath = SaveFileTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(filePath)) {
            set_validation_message("Для продолжения игры укажите путь к существующему файлу сохранения.");
            return;
        }

        var validation = save_file_validator_t.Validate(filePath, serializers);
        if (!validation.is_valid) {
            set_validation_message(validation.get_message);
            return;
        }

        try {
            var serializer = serializers.First(candidate => candidate.get_format == validation.get_format);
            var game = serializer.load(filePath);
            open_game_window(game, serializer, filePath);
        } catch (Exception exception) {
            set_validation_message($"Не удалось загрузить сохранение: {exception.Message}");
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
            update_suggested_save_path();
            set_validation_message("Папка сохранения обновлена.");
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
            set_validation_message("Файл сохранения выбран.");
        }
    }

    private void FormatComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        update_suggested_save_path();
    }

    private void open_game_window(chess_game_t game, IamInterfaceThatReperentsThatThisIsGameSerializer serializer, string saveFilePath) {
        var gameWindow = new GameWindow(game, serializer, saveFilePath);
        gameWindow.Closed += (_, _) => {
            Show();
            SaveFileTextBox.Text = saveFilePath;
            set_validation_message("Окно партии закрыто. При наличии ходов партия сохранена автоматически.");
        };

        gameWindow.Show();
        Hide();
    }

    private IamInterfaceThatReperentsThatThisIsGameSerializer get_selected_serializer() {
        var selected = FormatComboBox.SelectedItem as serialization_format_t?
            ?? serialization_format_t.SERIALIZATION_FORMAT_JSON;
        return serializers.First(serializer => serializer.get_format == selected);
    }

    private string get_save_file_path() {
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

        return Path.Combine(folderPath, $"autosave{get_selected_serializer().get_file_extension}");
    }

    private void update_suggested_save_path() {
        if (string.IsNullOrWhiteSpace(SaveFileTextBox.Text)) {
            SaveFileTextBox.Text = get_save_file_path();
        }
    }

    private void set_validation_message(string message) {
        ValidationTextBlock.Text = message;
    }
}
