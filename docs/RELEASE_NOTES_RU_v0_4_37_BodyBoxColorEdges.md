# Inventor IPT Organizer v0.4.37 — BodyBoxColorEdges

## Что исправлено

В v0.4.36 `Show / Hide body RangeBoxes` строил 8 RangeBox, но визуально были видны в основном тонкие контуры, а случайные цвета почти не различались.

## Что сделано

- Каждый `SurfaceBody.RangeBox` теперь получает яркий толстый контур по 12 рёбрам.
- Контуры строятся через `CurveGraphics`.
- Цвета берутся из high-contrast palette по индексу тела.
- Прозрачная face-заливка оставлена только как лёгкая подсказка объёма.
- По умолчанию body mode теперь `Cube hit: filtered bodies`.

## Новое событие лога

`BODY_RANGEBOXES_PREVIEW_BUILT` теперь пишет:

- `VisualMode=HighContrastColoredEdgesAndLightFaces`
- `ColorMode=HighContrastPaletteByBodyIndex`
- `EdgeLineWeight=3.5`
- `EdgeGraphics=...`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
