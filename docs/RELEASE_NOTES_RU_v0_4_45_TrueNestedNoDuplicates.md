# Inventor IPT Organizer v0.4.45 — TrueNestedNoDuplicates

## Что исправлено

v0.4.42/v0.4.44 создавали внешнюю папку из feature nodes, а потом внутреннюю папку снова из тех же feature nodes. Inventor показывал это как две папки с одинаковыми элементами.

## Новая логика

Для `Create feature browser folder`:

1. Создаётся внутренняя папка:
   `Selected_Features_Items_yyyyMMdd_HHmmss`
2. В неё помещаются feature nodes.
3. Программа получает BrowserNode этой внутренней папки.
4. Создаётся внешняя папка:
   `Selected_Features_yyyyMMdd_HHmmss`
5. Во внешнюю папку помещается только BrowserNode внутренней папки.

Если BrowserNode внутренней папки получить нельзя, программа оставляет только внутреннюю папку и не создаёт внешнюю, чтобы не создавать дубли.

## Новые логи

- `FEATURE_BROWSER_FOLDER_TRUE_NESTED_CREATED`
- `FEATURE_BROWSER_FOLDER_TRUE_NESTED_INNER_ONLY_FALLBACK`
- `FEATURE_BROWSER_FOLDER_INNER_NODE_RESOLVE_FAILED`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
