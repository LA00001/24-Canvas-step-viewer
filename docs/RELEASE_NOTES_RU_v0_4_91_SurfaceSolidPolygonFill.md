# v0.4.91 — SurfaceSolidPolygonFill

## Что исправляем

В v0.4.90 лог показывал:

- `Mode=Surface`
- `DrawnTriangles=23696`
- `SurfaceFillMode=Winding`

Но визуально был только bounding параллелепипед. Значит общий `GraphicsPath.FillPath` всё равно не давал видимой поверхности на этом STEP.

## Что сделано

- `Mode=Surface` теперь рисует каждый triangle напрямую через `g.FillPolygon`.
- `Surface+Edges` тоже использует прямую заливку + optional edge path.
- `FastDragWireframe` оставлен прежним быстрым.
- Лог теперь пишет:
  - `SurfaceFillMode=SolidFillPolygon`
  - `GdiStrategy=DirectFillPolygonPlusOptionalEdgePath`

## Цель

Сначала получить честно видимую поверхность. После этого можно оптимизировать и добавлять shading/depth sort.
