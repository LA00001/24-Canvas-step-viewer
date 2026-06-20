# Inventor IPT Organizer v0.4.62 — FastCacheView

## Что изменено

`Refresh tree #2 BASE` больше не строит BrowserPane snapshot.

Это cache-only режим:

- берёт готовый `BrowserTreeNamesOnlySnapshot`;
- обновляет DataGridView in-place;
- не вызывает live BrowserPane traversal;
- не вызывает live name sync;
- не читает `NativeObject`, `ObjectKind`, `XYZ` из BrowserNode.

## Если cache отсутствует

Кнопка пробует построить snapshot из текущего DataGridView.  
Если таблица пустая, показывает сообщение и не запускает скрытый медленный обход.

## Spatial cubes BASE

Если spatial base не готова, её можно пересчитать автоматически: 2x2x2 = 8 кубиков.  
Это разрешённый быстрый пересчёт и он не является BrowserPane traversal.

## Цель

Вернуть смысл v0.4.57/v0.4.59 cache-hit скорости, но сохранить X/Y/Z/Cubes/Cube IDs из уже готовой модели.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
