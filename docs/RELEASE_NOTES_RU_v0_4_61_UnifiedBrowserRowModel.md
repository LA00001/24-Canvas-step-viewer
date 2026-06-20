# Inventor IPT Organizer v0.4.61 — UnifiedBrowserRowModel

## Главная идея

`#2 BASE` больше не должен терять данные и потом восстанавливать их кусками.

Теперь первый `#2 BASE` строит единую row-model:

`BrowserNode -> BrowserTreeGridItem -> optional SurfaceBody -> SpatialBodyRecord`

## Что получает строка body сразу

- live BrowserNode name
- NativeObject / SurfaceBody
- CanMove
- CanRename
- X/Y/Z из cached SpatialBodyRecord.BodyBox
- Cubes / Cube IDs через существующий ApplySpatialCubesInfoToBrowserTreeRow

## Cache hit

На cache hit:

- структура берётся из row-model;
- X/Y/Z/Cubes/Cube IDs уже есть в row-model;
- live name sync обновляет только имена.

## Почему это лучше

Это не patch по симптомам, а смена источника правды: одна модель строк вместо цепочки names-only -> previous-grid merge -> spatial-name parsing.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
