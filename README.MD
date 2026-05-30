# MISIS Chess OOP

**Chess** - настольное приложение для игры в шахматы на двух игроков с возможностью сохранения и загрузки партий. Проект выполнен в рамках курса "Объектно-ориентированное программирование" (МИСИС)

## Особенности репозитория

Файл [AGENTS.md](AGENTS.md) представляет собой "намордник" для стохастического попугая (LLM-ассистента): строгие ограничения на то, что не можно и нельзя делать. Это гарантирует, что автоматические предложения не нарушат хрупкое душевное состояние одного из обоих из авторов.

---

## Таблица возможностей проекта

| Возможность                                     | Примечание                                                                                                                                                                                                                                                                                                           |
| ----------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| + Интерфейс фигуры, абстрактный класс `piece_t` | [IamInterfaceThatReperentsThatThisIsPiece](chess/Model/Core/IamInterfaceThatReperentsThatThisIsPiece.cs), [piece_t](chess/Model/Core/piece_t.cs)                                                                                                                                                                     |
| + Все 6 фигур                                   | [king_piece_t](chess/Model/Core/king_piece_t.cs), [queen_piece_t](chess/Model/Core/queen_piece_t.cs), [rook_piece_t](chess/Model/Core/rook_piece_t.cs), [bishop_piece_t](chess/Model/Core/bishop_piece_t.cs), [knight_piece_t](chess/Model/Core/knight_piece_t.cs), [pawn_piece_t](chess/Model/Core/pawn_piece_t.cs) |
| + Стартовое меню                                | [MainWindow.axaml.cs](chess/Chess/MainWindow.axaml.cs)                                                                                                                                                                                                                                                               |
| + Игровое окно                                  | [GameWindow.axaml.cs](chess/Chess/GameWindow.axaml.cs)                                                                                                                                                                                                                                                               |
| + JSON / XML сериализация                       | [json_game_serializer_t](chess/Model/Data/json_game_serializer_t.cs), [xml_game_serializer_t](chess/Model/Data/xml_game_serializer_t.cs), [game_serializer_t](chess/Model/Data/game_serializer_t.cs)                                                                                                                 |
| + Автосохранение при закрытии                   | `GameWindow_OnClosing`                                                                                                                                                                                                                                                                                               |
| + Шах, мат, пат (6 обратных ходов)              | [chess_game_t.checkmate.cs](chess/Model/Core/chess_game_t.checkmate.cs), [chess_game_t.stalemate.cs](chess/Model/Core/chess_game_t.stalemate.cs)                                                                                                                                                                     |
| + Сообщения о шахе/мате                         | Статусная строка                                                                                                                                                                                                                                                                                                     |
| + Подсветка доступных клеток                    | Зелёный цвет                                                                                                                                                                                                                                                                                                         |
| + Отмена выбора повторным кликом                | `ClearSelection`                                                                                                                                                                                                                                                                                                     |
| + Выбор папки сохранения                        | Кнопка «Выбрать папку»                                                                                                                                                                                                                                                                                               |
| + Валидация файла продолжения                   | [save_file_validator_t](chess/Model/Data/save_file_validator_t.cs)                                                                                                                                                                                                                                                   |
| + Рокировка и взятие на проходе                 | Логика в `king_piece_t` и `pawn_piece_t`                                                                                                                                                                                                                                                                             |
| + Partial классы (мат/пат)                      | Файлы `.checkmate.cs` и `.stalemate.cs`                                                                                                                                                                                                                                                                              |
| + Папки Core/Data                               | +                                                                                                                                                                                                                                                                                                                    |
| + Перегрузка операторов                         | `position_t + board_vector_t`                                                                                                                                                                                                                                                                                        |
| + Обобщения                                     | `grid_mapping_t<T>`, `pieces_getall<T>`                                                                                                                                                                                                                                                                              |
| + Делегаты                                      | `StateChanged`                                                                                                                                                                                                                                                                                                       |

---

## Технологии

| Компонент    | Технология                                   |
| ------------ | -------------------------------------------- |
| Язык         | C# 12                                        |
| UI           | Avalonia UI 12.0.3 (разрешён преподавателем) |
| .NET         | 9 (приложение) / 8 (библиотека Model)        |
| Сериализация | System.Text.Json / System.Xml.Serialization  |
| Сборка       | dotnet CLI / Makefile                        |

## Структура проекта

| Путь                | Содержимое                                         |
| ------------------- | -------------------------------------------------- |
| `chess/Chess/`      | Пользовательский интерфейс (Avalonia)              |
| `chess/Model/Core/` | Бизнес-логика: фигуры, доска, правила, шах/мат/пат |
| `chess/Model/Data/` | Сериализация, валидация, снапшоты                  |
| `Makefile`          | Сборка и запуск                                    |

## Ultra-Based сборка и запуск

```bash
make deps    # установка .NET SDK (pacman -S dotnet-sdk)
make build   # сборка решения
make run     # запуск приложения
make clean   # очистка артефактов
make fmt     # форматирование кода
```

<details>

<summary>

Соответствие требованиям проекта

</summary>

### Базовая часть

| Требование                                                | Реализация                                                                                            | Где искать                                                                                                                        |
| --------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Интерфейс с методами движения/атаки                       | + `IamInterfaceThatReperentsThatThisIsPiece` (методы `get_available_moves`, `can_attack`)             | [Model/Core/IamInterfaceThatReperentsThatThisIsPiece.cs](chess/Model/Core/IamInterfaceThatReperentsThatThisIsPiece.cs)            |
| Абстрактный класс фигуры                                  | + `piece_t` (абстрактный)                                                                             | [Model/Core/piece_t.cs](chess/Model/Core/piece_t.cs)                                                                              |
| Классы-наследники (6 видов)                               | + `king_piece_t`, `queen_piece_t`, `rook_piece_t`, `bishop_piece_t`, `knight_piece_t`, `pawn_piece_t` | [Model/Core/](chess/Model/Core/)                                                                                                  |
| Стартовое меню с кнопками «Новая игра», «Продолжить игру» | +                                                                                                     | [chess/Chess/MainWindow.axaml.cs](chess/Chess/MainWindow.axaml.cs)                                                                |
| Игровое окно с доской 8×8, управление мышью               | +                                                                                                     | [chess/Chess/GameWindow.axaml.cs](chess/Chess/GameWindow.axaml.cs)                                                                |
| Сериализация в JSON, сохранение при закрытии окна         | +                                                                                                     | [chess/Model/Data/json_game_serializer_t.cs](chess/Model/Data/json_game_serializer_t.cs), вызов `save()` в `GameWindow.OnClosing` |

### Дополнительная часть

| Требование                                  | Реализация                                                                  | Где искать                                                                               |
| ------------------------------------------- | --------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| Библиотека `Model`, отделение бизнес-логики | + Проект `Model`, папки `Core` и `Data`                                     | [chess/Model/](chess/Model/)                                                             |
| Partial класс для мата                      | + `chess_game_t` – partial, метод `is_checkmate` в отдельном файле          | [chess/Model/Core/chess_game_t.checkmate.cs](chess/Model/Core/chess_game_t.checkmate.cs) |
| Partial класс для пата (6 обратных ходов)   | + `is_stalemate` + `is_six_reversible_half_moves_reached` в отдельном файле | [chess/Model/Core/chess_game_t.stalemate.cs](chess/Model/Core/chess_game_t.stalemate.cs) |
| Сообщения о шахе и мате                     | + Отображаются в статусной строке                                           | `GetGameStateText()` в [GameWindow.axaml.cs](chess/Chess/GameWindow.axaml.cs)            |
| Подсветка доступных клеток                  | + При выборе фигуры зелёным цветом                                          | `GetCellBackground()` там же                                                             |
| Отмена выбора фигуры повторным кликом       | + Повторный клик по той же клетке – сброс выделения                         | `HandleCellClick()`                                                                      |
| Выбор папки для сохранения                  | + Кнопка «Выбрать папку»                                                    | [MainWindow.axaml.cs](chess/Chess/MainWindow.axaml.cs)                                   |

### Продвинутая часть (2 балла)

| Требование                                                      | Реализация                                                                                            | Где искать                                                                                                                                            |
| --------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Валидация файла продолжения (кнопка неактивна при ошибке)       | + При клике проверяется; кнопка динамически не деактивируется, но пользователь получает сообщение     | [save_file_validator_t.cs](chess/Model/Data/save_file_validator_t.cs)                                                                                 |
| Папки `Core` и `Data`                                           | +                                                                                                     | [chess/Model/Core](chess/Model/Core), [chess/Model/Data](chess/Model/Data)                                                                            |
| Сериализация в XML, абстрактный базовый класс                   | + `game_serializer_t` (абстрактный), `xml_game_serializer_t`                                          | [chess/Model/Data/game_serializer_t.cs](chess/Model/Data/game_serializer_t.cs), [xml_game_serializer_t.cs](chess/Model/Data/xml_game_serializer_t.cs) |
| Выпадающий список выбора формата, копирование при смене формата | + Комбобокс JSON/XML, при смене обновляется предлагаемый путь; интерфейс для копирования предоставлен | [MainWindow.axaml.cs](chess/Chess/MainWindow.axaml.cs)                                                                                                |

---

## Технические критерии для оценки

| Критерий                            | В проекте                                                                                                                                                     |
| ----------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Интерфейсов >=2                     | `IamInterfaceThatReperentsThatThisIsPiece` и `IamInterfaceThatReperentsThatThisIsGameSerializer`                                                              |
| Абстрактных классов >=2             | `piece_t`, `game_serializer_t`                                                                                                                                |
| Приведений (cast) >5                | В `game_snapshot_mapper_t.create_piece` (switch с приведением), в `is_king_in_check` (приведение к `king_piece_t`), в `get_legal_moves` и др.                 |
| Перегрузка методов >=1              | Конструкторы `chess_game_t`, перегрузка `save` в `game_serializer_t`, метод `TryMove`                                                                         |
| Переопределение методов/свойств >=3 | В каждой фигуре переопределены `get_type`, `get_symbol`, `get_available_moves`, `make_clone`; в сериализаторах – `serialize_snapshot`, `deserialize_snapshot` |
| Перегрузка операторов >=1           | `position_t operator + (position_t, board_vector_t)`                                                                                                          |
| Делегаты (встроенные или свои) >=1  | `Action<game_state_status_t> StateChanged`                                                                                                                    |
| Обобщения (generics) >=2            | `grid_mapping_t<T>`, метод `pieces_getall<T>()`                                                                                                               |
| Разных UI-элементов >3              | В главном окне: кнопки, комбобокс, текстовые поля, лейблы. В игровом окне: кнопки, грид, текстблоки.                                                          |


</details>

## Важные замечания

- Рокировка и анпассан реализованы.
- Пат по 6 обратным ходам (три пары туда-обратно).
- Автосохранение при закрытии окна игры.
- Выбор папки и формата сохранения (JSON/XML).

## Лицензия

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)  
Проект распространяется под лицензией **GNU General Public License v3.0 или новее** (SPDX: `GPL-3.0-or-later`). Файл [LICENSE](LICENSE).
