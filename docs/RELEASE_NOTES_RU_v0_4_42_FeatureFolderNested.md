# Inventor IPT Organizer v0.4.42 — FeatureFolderNested

## Что изменено

Кнопка `Create feature browser folder` теперь при первом создании делает сразу вложенную структуру:

- внешняя папка: `Selected_Features_yyyyMMdd_HHmmss`;
- внутренняя папка: `Selected_Features_Items_yyyyMMdd_HHmmss`;
- элементы/features перемещаются во внутреннюю папку.

Кнопка `Create feature browser folder #2` остаётся копией первой кнопки и вызывает тот же обработчик.

## Лог

Добавлено событие:

`FEATURE_BROWSER_FOLDER_NESTED_INNER_CREATED`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
