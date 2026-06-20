# Inventor IPT Organizer v0.4.59 — CacheNameSyncCompileFix

## Исправлено

v0.4.58 не компилировался, потому что в коде был вызов:

`PopulateIptBrowserTreeGridInPlaceFromItems`

но сам метод отсутствовал в пакете.

## Добавлено обратно

- `PopulateIptBrowserTreeGridInPlaceFromItems`
- `ApplyEditableStateToBrowserTreeRow`

## Логика сохранена

- `#2 BASE` синхронизирует live names из `BrowserNode`;
- X/Y/Z не очищаются;
- DataGridView обновляется in-place при совпадающем количестве строк;
- fallback на полное заполнение остаётся.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
