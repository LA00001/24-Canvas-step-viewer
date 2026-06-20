# Inventor IPT Organizer v0.4.70 — PrimitiveMouse3DViewer

## Кнопки

Кнопки `.ipt canvas` уменьшены до toolbar-формата 32x32. Значки рисуются программно как bitmap icons. Подробное описание остаётся в ToolTip.

## Локальный 3D viewer

Правая preview-область теперь умеет строить собственную примитивную 3D-модель по данным Spatial BASE:

- ModelBox
- 8 spatial cubes
- Body RangeBoxes

## Мышь

- ЛКМ drag — вращать
- колесо — zoom
- double-click — сброс камеры

## Важно

Это не точный BRep/Inventor viewport. Это быстрый локальный viewer по RangeBox/BodyBox primitive data.
