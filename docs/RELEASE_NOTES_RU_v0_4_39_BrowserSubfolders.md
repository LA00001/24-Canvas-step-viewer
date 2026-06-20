# Inventor IPT Organizer v0.4.39 — BrowserSubfolders

## Что добавлено

Добавлена кнопка:

`Create subfolder from selected nodes`

## Для чего

После создания папки через `Create feature browser folder` можно выделить внутренние элементы/features этой папки и создать из них новую подпапку.

## Источники выбранных узлов

Программа пробует получить BrowserNode из:

1. выделенных строк внутренней таблицы `Browser tree`;
2. `Inventor SelectSet`;
3. выбранных объектов активного `BrowserPane`;
4. fallback-сканирования `BrowserNode.Selected`.

## Скорость

После создания подпапки полный `RefreshIptBrowserTree` не запускается. Выполняется только обновление BrowserPane и лёгкое обновление caption.

## Лог

Добавлены события:

- `BROWSER_SUBFOLDER_SELECTED_NODE_SOURCE_SCAN`
- `BROWSER_SUBFOLDER_CREATED_FROM_SELECTED_NODES`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
