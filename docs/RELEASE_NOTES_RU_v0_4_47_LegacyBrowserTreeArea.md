# Inventor IPT Organizer v0.4.47 — LegacyBrowserTreeArea

## Что добавлено

Добавлена отдельная legacy-область:

`Legacy browser tree / старое дерево браузера Inventor`

Она взята по смыслу из старого проекта:

`MyFirstInventorPlugin_VS2017_Lesson5_CSharp_Tabs_IPT_IAM_BROWSER_TREE_EXPORT_FIX_IO_NAMES`

## Важно

Новая текущая область `Browser tree / дерево браузера Inventor` не заменена.

Теперь на `.ipt` вкладке есть две области:

1. Текущая редактируемая `Browser tree` таблица DataGridView.
2. Legacy TreeView-область из старого проекта.

## Кнопки

- `Refresh legacy browser tree`
- `Copy legacy tree to clipboard`
- `Save legacy tree JSON file`

## Почему так

Пользователь попросил не заменять существующую область, а добавить старую TreeView-область как отдельный блок.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
