# Inventor IPT Organizer v0.4.57 — DataGridInPlaceCompileFix

## Исправлено

v0.4.56 не компилировался из-за отсутствующих helper-методов и неправильного имени константы.

## Что исправлено

- добавлен `CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge`
- добавлен `MergeEditableBrowserTreeDataByRow`
- добавлен `CloneBrowserTreeGridItem`
- добавлены helper-методы сравнения строк
- `BrowserGridColLevel` заменён на `BrowserGridColDepth`

## Логика

`Refresh tree #2 BASE` по-прежнему должен:

- сохранять X/Y/Z;
- сохранять editable/movable строки;
- обновлять DataGridView in-place, если row count совпадает.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
