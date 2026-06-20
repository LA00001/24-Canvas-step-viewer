# v0.4.92 — SurfaceDepthShade

## Что исправляем

v0.4.91 доказал, что Surface уже реально рисуется, но вид получился как серый монолит.  
Причина: все triangles заливались одним цветом без depth-sort и shading.

## Что сделано

- Surface triangles собираются в draw-list.
- Добавлен view-space depth.
- Draw-list сортируется painter algorithm: дальние сначала, ближние потом.
- Для каждого triangle считается pseudo-normal в view-space.
- Цвет triangle получает простой shade от pseudo-light.
- Surface+Edges использует тот же shaded surface + edge path.
- FastDrag остаётся быстрым wireframe.

## Новые логи

- `STEP_SURFACE_SHADE_STATS`
- `SurfaceFillMode=DepthSortedShadedPolygons`
- `GdiStrategy=DepthSortedFillPolygonPseudoShade`

## Цель

Сделать Surface не серым монолитом, а читаемой объёмной формой.
