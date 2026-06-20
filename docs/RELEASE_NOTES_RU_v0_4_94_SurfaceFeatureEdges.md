# v0.4.94 — SurfaceFeatureEdges

## Что исправляем

v0.4.93 сделал opaque visible shell, но CAD-форма всё ещё слабо читалась без рёбер.

## Что сделано

- `Surface+Edges` теперь режим по умолчанию.
- Поверх visible shell рисуются не все triangle edges, а только feature edges:
  - boundary edges;
  - crease edges по различию normals.
- `Surface` остаётся чистой поверхностью.
- `Ghost` остаётся отдельным прозрачным режимом.
- `Wire` остаётся диагностическим режимом.
- Добавлен лог `STEP_FEATURE_EDGES_BUILT`.

## Новые логи

- `STEP_FEATURE_EDGES_BUILT`
- `DrawnFeatureEdges`
- `BoundaryEdges`
- `CreaseEdges`
- `EdgeMode=BoundaryAndCrease`

## Цель

Приблизить вид к Inventor: поверхность + читаемые CAD-рёбра.
