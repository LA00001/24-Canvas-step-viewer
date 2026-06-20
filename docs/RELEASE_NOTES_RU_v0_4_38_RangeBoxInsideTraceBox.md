# Inventor IPT Organizer v0.4.38 — RangeBoxInsideTraceBox

## Что добавлено

Добавлен третий режим отбора тел во вкладке `.ipt cubes`:

`3. RangeBox fully inside TraceBox`

## Логика режима

1. Рамка выбирает задетые spatial-cubes.
2. Из них берутся candidate bodies.
3. Для каждого candidate body проверяется `SurfaceBody.RangeBox`.
4. Тело остаётся в visible-list только если весь `RangeBox` полностью лежит внутри зелёного `Selection Trace Box`.

## Режимы dropdown

- `1. Cube hit: all bodies`
- `2. Cube hit: filtered bodies`
- `3. RangeBox fully inside TraceBox`

По умолчанию выбран `2. Cube hit: filtered bodies`.

## Лог

Добавлено событие:

`RANGEBOX_FULLY_INSIDE_TRACEBOX_FILTER_USED`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
